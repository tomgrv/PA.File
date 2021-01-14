using System;
using System.IO;
using System.Text;

namespace PA.File.Readers
{
    public class MFCBinaryReader : BinaryReader
    {
        private const ushort wNullTag = 0; // special tag indicating NULL ptrs
        private const ushort wNewClassTag = 0xFFFF; // special tag indicating new CRuntimeClass
        private const ushort wClassTag = 0x8000; // 0x8000 indicates class tag (OR'd)
        private const uint dwBigClassTag = 0x80000000; // 0x8000000 indicates big class tag (OR'd)
        private const ushort wBigObjectTag = 0x7FFF; // 0x7FFF indicates DWORD object tag
        private const uint nMaxMapCount = 0x3FFFFFFE; // 0x3FFFFFFE last valid mapCount

        public MFCBinaryReader(Stream s)
            : base(s)
        {
        }

        public MFCBinaryReader(Stream s, Encoding e)
            : base(s, e)
        {
        }

        public override bool ReadBoolean()
        {
            return ReadInt32().Equals(1);
        }

        public override string ReadString()
        {
            return ReadString("");
        }

        public string ReadString(string strDefault)
        {
            var strResult = "";
            var nConvert = 1;

            var nNewLen = ReadStringLength();
            if (nNewLen == unchecked((uint) -1))
            {
                nConvert = 1 - nConvert;
                nNewLen = ReadStringLength();
                if (nNewLen == unchecked((uint) -1)) return strResult;
            }

            // set length of string to new length
            var nByteLen = nNewLen;

            // bytes to read
            nByteLen += (uint) (nByteLen * (1 - nConvert));

            // read in the characters
            if (nNewLen != 0)
            {
                // read new data
                var byteBuf = ReadBytes((int) nByteLen);

                // convert the data if as necessary
                var sb = new StringBuilder();
                if (nConvert != 0)
                    for (var i = 0; i < nNewLen; i++)
                        sb.Append((char) byteBuf[i]);
                else
                    for (var i = 0; i < nNewLen; i++)
                        sb.Append((char) (byteBuf[i * 2] + byteBuf[i * 2 + 1] * 256));

                strResult = sb.ToString();
            }

            if (strResult.Length > 0)
                return strResult;
            return strDefault;
        }

        public object ReadObject()
        {
            uint nSchema = 0;
            ulong obTag = 0;

            var oClass = ReadClass(ref nSchema, ref obTag);

            return null;
        }

        public Type ReadClass(ref uint nSchema, ref ulong obTag)
        {
            // read object tag - if prefixed by wBigObjectTag then DWORD tag follows

            var wTag = ReadUInt16();

            if (wTag == wBigObjectTag) obTag = ReadUInt64();

            // check for object tag (throw exception if expecting class tag)

            //Type ClassRef;
            // UINT nSchema;
            if (wTag == wNewClassTag)
                RuntimeClassLoad(1);

            //     // new object follows a new class id
            //     if ((pClassRef = CRuntimeClass::Load(*this, &nSchema)) == NULL)
            //         AfxThrowArchiveException(CArchiveException::badClass, m_strFileName);

            //     // check nSchema against the expected schema
            //     if ((pClassRef->m_wSchema & ~VERSIONABLE_SCHEMA) != nSchema)
            //     {
            //         if (!(pClassRef->m_wSchema & VERSIONABLE_SCHEMA))
            //         {
            //             // schema doesn't match and not marked as VERSIONABLE_SCHEMA
            //             AfxThrowArchiveException(CArchiveException::badSchema,
            //                 m_strFileName);
            //         }
            //         else
            //         {
            //             // they differ -- store the schema for later retrieval
            //             if (m_pSchemaMap == NULL)
            //                 m_pSchemaMap = new CMapPtrToPtr;
            //             ASSERT_VALID(m_pSchemaMap);
            //             m_pSchemaMap->SetAt(pClassRef, (void*)(DWORD_PTR)nSchema);
            //         }
            //     }
            //     CheckCount();
            //     m_pLoadArray->InsertAt(m_nMapCount++, pClassRef);
            // else
            // {
            //     // existing class index in obTag followed by new object
            //     DWORD nClassIndex = (obTag & ~dwBigClassTag);
            //     if (nClassIndex == 0 || nClassIndex > (DWORD)m_pLoadArray->GetUpperBound())
            //         AfxThrowArchiveException(CArchiveException::badIndex,
            //             m_strFileName);

            //     pClassRef = (CRuntimeClass*)m_pLoadArray->GetAt(nClassIndex);
            //     ASSERT(pClassRef != NULL);

            //     // determine schema stored against objects of this type
            //     void* pTemp;
            //     BOOL bFound = FALSE;
            //     nSchema = 0;
            //     if (m_pSchemaMap != NULL)
            //     {
            //         bFound = m_pSchemaMap->Lookup( pClassRef, pTemp );
            //         if (bFound)
            //             nSchema = (UINT)(UINT_PTR)pTemp;
            //     }
            //     if (!bFound)
            //         nSchema = pClassRef->m_wSchema & ~VERSIONABLE_SCHEMA;
            //}

            // // check for correct derivation
            // if (pClassRefRequested != NULL &&
            //     !pClassRef->IsDerivedFrom(pClassRefRequested))
            // {
            //     AfxThrowArchiveException(CArchiveException::badClass, m_strFileName);
            // }

            // // store nSchema for later examination
            // if (pSchema != NULL)
            //     *pSchema = nSchema;
            // else
            //     m_nObjectSchema = nSchema;

            // // store obTag for later examination
            // if (pObTag != NULL)
            //     *pObTag = obTag;

            // // return the resulting CRuntimeClass*
            // return pClassRef;

            return null;
        }

        private void RuntimeClassLoad(uint pwSchemaNum)
        {
            pwSchemaNum = ReadUInt32();
            var nLen = ReadUInt16();

            var strClassName = ReadChars(nLen).ToString();

            //// load the class name
            //if (nLen >= _countof(szClassName) ||
            //    ar.Read(szClassName, nLen*sizeof(char)) != nLen*sizeof(char))
            //{
            //    return NULL;
            //}
            //szClassName[nLen] = '\0';

            //// match the string against an actual CRuntimeClass
            //CRuntimeClass* pClass = FromName(szClassName);
            //if (pClass == NULL)
            //{
            //    // not found, trace a warning for diagnostic purposes
            //    TRACE(traceAppMsg, 0, "Warning: Cannot load %hs from archive.  Class not defined.\n",
            //        szClassName);
            //}

            //return pClass;
        }

        private uint ReadStringLength()
        {
            // attempt BYTE length first
            var bLen = ReadByte();

            if (bLen < 0xff)
                return bLen;

            // attempt WORD length
            var wLen = ReadUInt16();

            if (wLen == 0xfffe)
                // UNICODE string prefix (length will follow)
                return unchecked((uint) -1);

            if (wLen == 0xffff)
                // read DWORD of length
                return ReadUInt32();

            return wLen;
        }
    }
}