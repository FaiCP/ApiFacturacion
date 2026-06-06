using Domain.Interfaces;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace Infrastructure.Services;

/// <summary>
/// Firma XML con XAdES-BES usando certificado .p12 — estándar SRI Ecuador
/// </summary>
public class FirmaDigitalService : IFirmaDigitalService
{
    private const string C14N_URL       = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
    private const string SHA1_URL       = "http://www.w3.org/2000/09/xmldsig#sha1";
    private const string RSASHA1_URL    = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
    private const string ENVELOPED_URL  = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
    private const string XADES_NS       = "http://uri.etsi.org/01903/v1.3.2#";
    private const string DSIG_NS        = "http://www.w3.org/2000/09/xmldsig#";
    private const string XADES_SIGNED_PROPS_TYPE = "http://uri.etsi.org/01903#SignedProperties";

    public Task<string> FirmarXmlAsync(string xml, string certificadoBase64, string password)
    {
        var certBytes = Convert.FromBase64String(certificadoBase64);
        using var cert = new X509Certificate2(certBytes, password,
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet);
        using var rsa = cert.GetRSAPrivateKey()
            ?? throw new InvalidOperationException("El certificado no contiene clave privada RSA.");

        var uid = Guid.NewGuid().ToString("N")[..8];
        var sigId       = $"Signature{uid}";
        var certRefId   = $"Certificate{uid}";
        var signedPropsId = $"SignedProperties{uid}";
        var objectId    = $"QualifyingProperties{uid}";

        // Parse XML original
        var doc = new XmlDocument { PreserveWhitespace = true };
        doc.LoadXml(xml);

        // ── 1. KeyInfo element ────────────────────────────────────────────
        var certB64 = Convert.ToBase64String(cert.RawData);
        var keyInfoXml = $"""
            <ds:KeyInfo xmlns:ds="{DSIG_NS}" Id="{certRefId}">
              <ds:X509Data>
                <ds:X509Certificate>{certB64}</ds:X509Certificate>
              </ds:X509Data>
            </ds:KeyInfo>
            """;
        var keyInfoDoc = LoadXmlDoc(keyInfoXml);
        var keyInfoDigest = DigestC14N(keyInfoDoc.DocumentElement!);

        // ── 2. SignedProperties (XAdES) ───────────────────────────────────
        var certSha1B64 = Convert.ToBase64String(SHA1.HashData(cert.RawData));
        var signingTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        var signedPropsXml = $"""
            <xades:SignedProperties xmlns:xades="{XADES_NS}" xmlns:ds="{DSIG_NS}" Id="{signedPropsId}">
              <xades:SignedSignatureProperties>
                <xades:SigningTime>{signingTime}</xades:SigningTime>
                <xades:SigningCertificate>
                  <xades:Cert>
                    <xades:CertDigest>
                      <ds:DigestMethod Algorithm="{SHA1_URL}"/>
                      <ds:DigestValue>{certSha1B64}</ds:DigestValue>
                    </xades:CertDigest>
                    <xades:IssuerSerial>
                      <ds:X509IssuerName>{EscapeXml(cert.IssuerName.Name)}</ds:X509IssuerName>
                      <ds:X509SerialNumber>{cert.SerialNumber}</ds:X509SerialNumber>
                    </xades:IssuerSerial>
                  </xades:Cert>
                </xades:SigningCertificate>
              </xades:SignedSignatureProperties>
            </xades:SignedProperties>
            """;
        var signedPropsDoc = LoadXmlDoc(signedPropsXml);
        var signedPropsDigest = DigestC14N(signedPropsDoc.DocumentElement!);

        // ── 3. Digest de comprobante (doc original sin signature, con enveloped transform) ──
        var comprobanteDigest = DigestComprobanteC14N(doc);

        // ── 4. SignedInfo ─────────────────────────────────────────────────
        var signedInfoXml = $"""
            <ds:SignedInfo xmlns:ds="{DSIG_NS}">
              <ds:CanonicalizationMethod Algorithm="{C14N_URL}"/>
              <ds:SignatureMethod Algorithm="{RSASHA1_URL}"/>
              <ds:Reference Id="Ref{uid}" URI="#comprobante">
                <ds:Transforms>
                  <ds:Transform Algorithm="{ENVELOPED_URL}"/>
                  <ds:Transform Algorithm="{C14N_URL}"/>
                </ds:Transforms>
                <ds:DigestMethod Algorithm="{SHA1_URL}"/>
                <ds:DigestValue>{comprobanteDigest}</ds:DigestValue>
              </ds:Reference>
              <ds:Reference URI="#{certRefId}">
                <ds:Transforms>
                  <ds:Transform Algorithm="{C14N_URL}"/>
                </ds:Transforms>
                <ds:DigestMethod Algorithm="{SHA1_URL}"/>
                <ds:DigestValue>{keyInfoDigest}</ds:DigestValue>
              </ds:Reference>
              <ds:Reference Type="{XADES_SIGNED_PROPS_TYPE}" URI="#{signedPropsId}">
                <ds:Transforms>
                  <ds:Transform Algorithm="{C14N_URL}"/>
                </ds:Transforms>
                <ds:DigestMethod Algorithm="{SHA1_URL}"/>
                <ds:DigestValue>{signedPropsDigest}</ds:DigestValue>
              </ds:Reference>
            </ds:SignedInfo>
            """;

        var signedInfoDoc = LoadXmlDoc(signedInfoXml);
        var signedInfoC14NBytes = C14NBytes(signedInfoDoc.DocumentElement!);
        var signatureBytes = rsa.SignData(signedInfoC14NBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        var signatureValueB64 = Convert.ToBase64String(signatureBytes);

        // ── 5. Signature completa ─────────────────────────────────────────
        var signatureXml = $"""
            <ds:Signature xmlns:ds="{DSIG_NS}" Id="{sigId}">
              {signedInfoXml}
              <ds:SignatureValue Id="SignatureValue{uid}">{signatureValueB64}</ds:SignatureValue>
              {keyInfoXml}
              <ds:Object Id="{objectId}">
                <xades:QualifyingProperties xmlns:xades="{XADES_NS}" Target="#{sigId}">
                  {signedPropsXml}
                </xades:QualifyingProperties>
              </ds:Object>
            </ds:Signature>
            """;

        var sigDoc = new XmlDocument { PreserveWhitespace = true };
        sigDoc.LoadXml(signatureXml);

        doc.DocumentElement!.AppendChild(doc.ImportNode(sigDoc.DocumentElement!, true));

        return Task.FromResult(doc.OuterXml);
    }

    private static string DigestC14N(XmlElement element)
    {
        var bytes = C14NBytes(element);
        return Convert.ToBase64String(SHA1.HashData(bytes));
    }

    private static string DigestComprobanteC14N(XmlDocument doc)
    {
        // Para el elemento raíz aplicamos enveloped-signature transform (no hay Signature aún)
        // seguido de C14N — como no existe Signature en el doc, C14N del elemento raíz es suficiente
        return DigestC14N(doc.DocumentElement!);
    }

    private static byte[] C14NBytes(XmlElement element)
    {
        var transform = new XmlDsigC14NTransform();
        var tempDoc = new XmlDocument { PreserveWhitespace = true };
        tempDoc.LoadXml(element.OuterXml);
        transform.LoadInput(tempDoc);
        using var stream = (MemoryStream)transform.GetOutput(typeof(MemoryStream));
        return stream.ToArray();
    }

    private static XmlDocument LoadXmlDoc(string xml)
    {
        var doc = new XmlDocument { PreserveWhitespace = true };
        doc.LoadXml(xml);
        return doc;
    }

    private static string EscapeXml(string s) =>
        s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
}
