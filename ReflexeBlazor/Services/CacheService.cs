using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Components.Forms;

namespace ReflexeBlazor.Services;

public class CacheService
{
    private Dictionary<string, Assembly> _assemblies;
    public Dictionary<string, Assembly> Assemblies => _assemblies;
    private readonly IHostEnvironment _env;
    private const string UPLOADS_PATH = "upload";

    public CacheService(IHostEnvironment env)
    {
        _assemblies = new Dictionary<string, Assembly>();
        _env = env;
        DeleteUploadedFiles();
    }

    private void DeleteUploadedFiles()
    {
        var filesToDel = Directory.GetFiles(UPLOADS_PATH);
        foreach (var file in filesToDel)
        {
            var ext = Path.GetExtension(file);
            if (ext == ".del")
            {
                File.Delete(file);
            }
        }
    }

    public async Task AddToCache(InputFileChangeEventArgs obj)
    {
        var id = $"{Guid.NewGuid()}{DateTime.UtcNow.Ticks}";
        if (!Directory.Exists(UPLOADS_PATH))
            Directory.CreateDirectory(UPLOADS_PATH);

        var path = $"{UPLOADS_PATH}/{id}.dll";

        using (Stream stream = File.Create(path))
        {
            await obj.File.OpenReadStream().CopyToAsync(stream);
            stream.Flush();
        }
        GetCachedAssemblies();
    }

    public void GetCachedAssemblies()
    {
        if (!Directory.Exists(UPLOADS_PATH))
            return;

        var paths = Directory.GetFiles(UPLOADS_PATH);
        var files = paths.Select(path => Path.GetFileName(path)).ToList();
        foreach (var file in files)
        {
            try
            {
                var filename = file.Split('.');
                if (filename.Last() == "dll")
                {
                    var path = $"{UPLOADS_PATH}/{file}";
                    _assemblies.Add(filename[0], Assembly.LoadFile($"{_env.ContentRootPath}/{path}"));
                }
            }
            catch (Exception e)
            {
                File.Delete(file);
            }
            
        }
    }

    public void RemoveFromCache(string id)
    {
        var path = $"{UPLOADS_PATH}/{id}.dll";
        var pathDel = $"{UPLOADS_PATH}/{id}.del";
        _assemblies.Clear();
        if (File.Exists(path))
        {
            File.Move(path, pathDel);
        }
        GetCachedAssemblies();
    }
}