namespace Jakar.Extensions;


public static class PathExtensions
{
    extension( DirectoryInfo    root )
    {
        public string[] Directories() => Directory.GetDirectories(root.FullName);
        public string[] FilePaths()   => Directory.GetFiles(root.FullName);
        public string[] SubFolderNames() => root.EnumerateDirectories()
                                                .Select(static x => x.Name)
                                                .ToArray();
        public string Combine( string                      fileName )   => Path.Combine(root.FullName, fileName);
        public string Combine( params ReadOnlySpan<string> subFolders ) => root.FullName.Combine(subFolders);
    }



    /// <param name="path"> </param>
    extension( string path )
    {
        public string Combine( params ReadOnlySpan<string> subFolders )
        {
            string[] results = GC.AllocateUninitializedArray<string>(subFolders.Length + 1);
            results[0] = path;
            for ( int i = 0; i < subFolders.Length; i++ ) { results[i + 1] = subFolders[i]; }

            return Path.Combine(results);
        }
        /// <summary> extension method for <see cref="Path.GetExtension(string)"/> </summary>
        /// <returns> </returns>
        public string GetExtension() => Path.GetExtension(path);
        /// <summary> extension method for <see cref="Path.GetFileName(string)"/> </summary>
        /// <returns> </returns>
        public string GetFileName() => Path.GetFileName(path);
        /// <summary> extension method for <see cref="Path.GetFileNameWithoutExtension(string)"/> </summary>
        /// <returns> </returns>
        public string GetFileNameWithoutExtension() => Path.GetFileNameWithoutExtension(path);
    }
}
