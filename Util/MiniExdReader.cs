using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using SaintCoinach;
using SaintCoinach.IO;

namespace Nophica
{
    class MiniExdReader
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct ExdHeader {
            private int sig;
            private short version;
            private short unk;
            private int offsetTableSize;
            private int dataSectionSize;
            private unsafe fixed byte padding[16];
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct OffsetTableEntry {
            private int index;
            private int offset;
        }

        private MiniExdReader() {}

        public static uint GetNumRows(ARealmReversed realm, string sheet) {
            byte[] file = GetSheet(realm, sheet);
            if (file == null)
                return 0;

            return GetNumRows(file);
        }

        public static uint GetNumRows(byte[] file) {
            return OrderedBitConverter.ToUInt32(file, 8, true) / 8;
        }

        public static byte[] GetRow(ARealmReversed realm, string sheet, int row) {

            byte[] file = GetSheet(realm, sheet);
            if (file == null)
                return null;

            return GetRow(file, row);
        }

        public static byte[] GetRow(byte[] file, int row) {
            int offset = 0;

            //ex header
            offset += 32;

            //seek to offset in table
            for (int i = 0; i < row; i++)
                offset += 8;
            offset += 4;

            int rowOffset = (int)OrderedBitConverter.ToUInt32(file, offset, true);
            uint length = OrderedBitConverter.ToUInt32(file, rowOffset, true);
            byte[] rowData = new byte[length];
            Array.Copy(file, rowOffset + 6, rowData, 0, length);

            return rowData;
        }

        private static byte[] GetSheet(ARealmReversed realm, string sheet) {
            return !realm.Packs.TryGetFile(sheet, out File file) ? null : file.GetData();
        }
    }
}
