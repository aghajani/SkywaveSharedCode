using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SSC.IO.Compression
{
    public interface IZipStorerProvider
    {

        Task<Stream> File_CreateStream(string filePath);

        Task<Stream> File_OpenStream(string filePath, bool needWriteAccess);

        Task<DateTime> File_GetLastWriteTime(string filePath);

        Task File_CloseStream(Stream stream);

        Task<string> Directory_GetFullName(string dirPath);

        Task<bool> Directory_Exists(string dirPath);

        Task Directory_Create(string dirPath);

        //Task Directory_Delete(string dirPath);

        Task File_SetCreationTime(string filePath, DateTime dateTime);

        Task File_SetLastWriteTime(string filePath, DateTime dateTime);

        Task<bool> Stream_IsFile(Stream stream);

        Task File_Delete(string filePath);

        Task File_Move(string pathSource, string pathDest);

        Task<string> Path_GetTempFileName();

        Task<bool> File_Exists(string filePath);

        Task<string> Directory_GetName(string dirPath);

        Task<IEnumerable<string>> Directory_GetFiles(string dirPath);

        Task<string> File_GetFullName(string filePath);

        Task<string> File_GetName(string filePath);

        Task<IEnumerable<string>> Directory_GetDirectories(string dirPath);
    }
}
