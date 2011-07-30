using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Virus
{
    using SpriteConfig = Dictionary<string, AnimationConfig>;
    using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

    public class AnimationConfig
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int FramesNum { get; set; }
        public bool Looping { get; set; }
        public Texture2D[] Textures { get; set; }
    }

    public class AnimationFactory
    {
        private Dictionary<string, SpriteConfig> _sprites { get; set; }

        public AnimationFactory(ContentManager contentManager, string xmlFilePath)
        {
            var xmlDB = XDocument.Load(xmlFilePath);

            _sprites = (from spriteConfig in xmlDB.Descendants("Sprite")
                        let spriteName = spriteConfig.Attribute("Name").Value
                        let anim = (from animationConfig in spriteConfig.Descendants("Animation")
                                    select new AnimationConfig()
                                   {
                                       Name = animationConfig.Attribute("Name").Value,
                                       FramesNum = int.Parse(animationConfig.Attribute("FramesNum").Value),
                                       Type = animationConfig.Attribute("Type").Value,
                                       Looping = bool.Parse(animationConfig.Attribute("Looping").Value),
                                       Textures = (from t in animationConfig.Descendants("Texture")
                                                  select contentManager.Load<Texture2D>(t.Attribute("Path").Value)).ToArray()
                                   }).ToDictionary(key => key.Name)
                        select new
                        {
                            key = spriteName,
                            value = anim
                        }).ToDictionary(item => item.key, item => item.value);
        }

        public Dictionary<string, Animation> CreateAnimations(string spriteName)
        {
            Dictionary<string, Animation> animationDictionary = new Dictionary<string, Animation>();

            foreach (var a in _sprites[spriteName])
            {
                switch (a.Value.Type)
                {
                    case "Simple":
                        animationDictionary.Add(a.Key, new SpriteSheetAnimation(a.Value.FramesNum, a.Value.Looping, a.Value.Textures[0]));
                        break;
                    
                    case "BigPortrait":
                        animationDictionary.Add(a.Key, new ScreenAnimation(a.Value.FramesNum, a.Value.Looping, true, a.Value.Textures));
                        break;

                    case "BigLandscape":
                        animationDictionary.Add(a.Key, new ScreenAnimation(a.Value.FramesNum, a.Value.Looping, false, a.Value.Textures));
                        break;

                    default:
                        throw new ArgumentException("FANCULO!", "spriteName");
                }
            }

            return animationDictionary;
        }
    }
}
