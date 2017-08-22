using System;
using System.Runtime.InteropServices;


class Win32API
{
    [StructLayout(LayoutKind.Sequential)]
    public class SHFILEINFO
    {
        public SHFILEINFO()
        {
            szDisplayName = new char[260];
            szTypeName = new char[80];
        }

        public IntPtr hIcon;
        public int iIcon;
        public long dwAttributes;
        public char[] szDisplayName;
        public char[] szTypeName;
    }

    [DllImport("shell32.dll")]
    public static extern UIntPtr SHGetFileInfo( string path,
                                                ulong fileAttr,
                                                ref SHFILEINFO psfi,
                                                uint cbFileInfo,
                                                uint uFlags);
 /*
        sFile:
            The name of an executable file, DLL, or icon file from which icons will be extracted.
        iIndex[in]:
            The zero-based index of the first icon to extract.

            If this value is –1 and phiconLarge and phiconSmall are both NULL, the function returns
            the total number of icons in the specified file.If the file is an executable file or DLL,
            the return value is the number of RT_GROUP_ICON resources.If the file is an.ico file,
            the return value is 1.
            If this value is a negative number and either phiconLarge or phiconSmall is not NULL, the
            function begins by extracting the icon whose resource identifier is equal to the absolute
            value of nIconIndex.For example, use -3 to extract the icon whose resource identifier is 3.
phLargeVersion[out, optional]:
            An array of icon handles that receives handles to the large icons extracted from the file.
            If this parameter is NULL, no large icons are extracted from the file.
phiconSmall[out, optional]:
            An array of icon handles that receives handles to the small icons extracted from the file.
            If this parameter is NULL, no small icons are extracted from the file.
nIcons[in]:
            The number of icons to be extracted from the file.
*/

    [DllImport("shell32.dll")]
    public extern static int ExtractIconEx(string sFile, int iIndex,
                                        out IntPtr piLargeVersion,
                                        out IntPtr piSmallVersion,
                                        int amountIcons);
    [DllImport("shell32.dll")]
    public extern static int SHDefExtractIcon(string sFile, int iIndex,
                                    uint flags,
                                    out IntPtr piLargeVersion,
                                    out IntPtr piSmallVersion,
                                    uint amountIcons);

    /*!
    * \return This function returns an icon or null
    */
    public static System.Drawing.Icon ExtractIcon(string file, int index, uint size, bool largeIcon)
    {
        IntPtr large;
        IntPtr small;
        //ExtractIconEx(file, number, out large, out small, 1);
        SHDefExtractIcon(file, index, 0, out large, out small, size);
        try
        {
            return System.Drawing.Icon.FromHandle(largeIcon ? large : small);
        }
        catch
        {
            return null;
        }
    }
}

