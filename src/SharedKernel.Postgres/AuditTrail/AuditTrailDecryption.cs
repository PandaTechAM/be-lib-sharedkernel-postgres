using AuditTrail.Abstractions;
using Pandatech.Crypto.Helpers;

namespace SharedKernel.Postgres.AuditTrail;

public class AuditTrailDecryption : IAuditTrailDecryption
{
   public string Decrypt(byte[]? cipherText, bool includesHash)
   {
      return includesHash
         ? Aes256.Decrypt(cipherText ?? [])
         : Aes256.DecryptWithoutHash(cipherText ?? []);
   }
}