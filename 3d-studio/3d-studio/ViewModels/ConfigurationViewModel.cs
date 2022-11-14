using Cells;
using Newtonsoft.Json;
using System;
using System.IO;

namespace WifViewer.ViewModels
{
    public class ConfigurationViewModel
    {
        public ConfigurationViewModel()
        {
            this.RayTracerPath = Cell.Create(Configuration.RAYTRACER_PATH);
            this.FfmpegPath = Cell.Create(Configuration.FFMPEG_PATH);
            this.AutoSave = Cell.Create(Configuration.AUTO_SAVE);

            IsRaytracerFound = Cell.Create(Path.GetFileNameWithoutExtension(Configuration.RAYTRACER_PATH) == "raytracer" && File.Exists(Configuration.RAYTRACER_PATH));
        }

        public Cell<string> RayTracerPath { get; }

        public Cell<string> FfmpegPath { get; }

        public Cell<bool> AutoSave { get; }

        public Cell<bool> IsRaytracerFound { get; }

        public void AcceptChanges()
        {
            Configuration.RAYTRACER_PATH = this.RayTracerPath.Value;
            Configuration.FFMPEG_PATH = this.FfmpegPath.Value;
            Configuration.AUTO_SAVE = this.AutoSave.Value;

            WifCfg wifCfg = new WifCfg
            {
                raytracer = Configuration.RAYTRACER_PATH,
                ffmpeg = Configuration.FFMPEG_PATH,
                block_size = 500000,
                auto_save = Configuration.AUTO_SAVE
            };

            var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "wifcfg.json");
            File.WriteAllText(file, JsonConvert.SerializeObject(wifCfg));
        }
    }

    public class WifCfg
    {
        public string raytracer { get; set; }
        public string ffmpeg { get; set; }
        public int block_size { get; set; }
        public bool auto_save { get; set; }
    }
}
