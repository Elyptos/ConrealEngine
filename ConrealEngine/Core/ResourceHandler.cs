/*
	Copyright (c) 2017 Thomas Schöngrundner

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.
*/

using ConrealEngine.Assets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ConrealEngine
{ 
    public class ResourceHandler : Submodule
    {
        private static readonly ResourceHandler instance = new ResourceHandler();

        public static ResourceHandler Instance { get { return instance; } }

        public override void Start()
        {
            if (IsActive)
                return;

            active = true;
        }

        public override void Stop()
        {
            if (!IsActive)
                return;

            active = false;
        }

        public UnicodeImage LoadImage(string imagePath)
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    using (Image img = Image.FromFile(imagePath))
                    {
                        return new UnicodeImage(new Bitmap(img));
                    }
                }
            }
            catch (IOException) { }

            return new UnicodeImage(new Bitmap(10, 10));
        }

        public bool SaveToJSON(object objToSave, string path)
        {
            string json = JsonConvert.SerializeObject(objToSave);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                File.WriteAllText(path, json);

                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public bool LoadFromJSON<T>(out T loadedObject, string path)
        {
            try
            {
                string json = File.ReadAllText(path);

                loadedObject = JsonConvert.DeserializeObject<T>(json);

                return true;
            }
            catch (IOException)
            {
                loadedObject = default(T);
                return false;
            }
            catch(JsonReaderException)
            {
                loadedObject = default(T);
                return false;
            }
        }
    }
}
