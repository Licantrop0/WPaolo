using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Virus
{

    public class AnimationConfig
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int FramesNum { get; set; }
        public bool Looping { get; set; }
        public string Origin { get; set; }
        public Texture2D[] Textures { get; set; }
    }

    public class SpritePrototypeContainer
    {
        private XElement[] _items;
        private int _position = -1;
        public int TotalItems { get; private set; }
        private Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

        public Dictionary<string, Sprite> Sprites
        {
            get { return _sprites; }
            set { _sprites = value; }
        }

        public SpritePrototypeContainer(ContentManager contentManager, string xmlFilePath)
        {
            _items = XDocument.Load(xmlFilePath).Descendants("Sprite").ToArray();
            TotalItems = _items.Length;
            _position = TotalItems > 0 ? 0 : -1;

            // se voglio il caricamento "poco alla volta" questa non deve essere chiamata, ma deve essere chiamata
            // dall'esterno la LoadCurrentElement
            LoadAllSprites(contentManager);
        }

        private void LoadAllSprites(ContentManager contentManager/*, XDocument xmlDB*/)
        {
            //_sprites = (from spriteConfig in xmlDB.Descendants("Sprite")
            //            let spriteName = spriteConfig.Attribute("Name").Value
            //            let anim = (from animationConfig in spriteConfig.Descendants("Animation")
            //                        select new AnimationConfig()
            //                        {
            //                            Name = animationConfig.Attribute("Name").Value,
            //                            FramesNum = int.Parse(animationConfig.Attribute("FramesNum").Value),
            //                            Type = animationConfig.Attribute("Type").Value,
            //                            Looping = bool.Parse(animationConfig.Attribute("Looping").Value),
            //                            Origin = animationConfig.Attribute("Origin").Value,
            //                            Textures = (from t in animationConfig.Descendants("Texture")
            //                                        select contentManager.Load<Texture2D>(t.Attribute("Path").Value)).ToArray()
            //                        }).ToDictionary(key => key.Name)
            //            select new
            //            {
            //                key = spriteName,
            //                value = anim
            //            }).ToDictionary(item => item.key, item => item.value);

            for (int i = 0; i < TotalItems; i++)
            {
                LoadCurrentElement(contentManager);
            }
        }

        // PS NON USATE, POSSONO SERVIRE PER IL CARICAMENTO "POCO ALLA VOLTA" PER BARRA DI CARICAMENTO 
        public bool LoadCurrentElement(ContentManager contentManager)
        {
            Dictionary<string, Animation> animationDictionary = new Dictionary<string, Animation>();
            var currentSprite = _items[_position];

            foreach (var anim in currentSprite.Descendants("Animation"))
            {
                var currentAnimation = new AnimationConfig()
                {
                    Name = anim.Attribute("Name").Value,
                    FramesNum = int.Parse(anim.Attribute("FramesNum").Value),
                    Type = anim.Attribute("Type").Value,
                    Looping = bool.Parse(anim.Attribute("Looping").Value),
                    Origin = anim.Attribute("Origin").Value,
                    Textures = (from t in anim.Descendants("Texture")
                                select contentManager.Load<Texture2D>(t.Attribute("Path").Value)).ToArray()
                };

                if (currentAnimation.Type == "Simple")
                {
                    animationDictionary.Add(currentAnimation.Name,
                        new SpriteSheetAnimation(currentAnimation.FramesNum,
                            currentAnimation.Looping,
                            currentAnimation.Textures[0]));
                }
                else
                {
                    bool isPortrait = false;

                    if (currentAnimation.Type == "BigPortrait")
                    {
                        isPortrait = true;
                    }
                    else if (currentAnimation.Type == "Landscape")
                    {
                        isPortrait = false;
                    }

                    if (currentAnimation.Origin == "default")
                    {
                        animationDictionary.Add(currentAnimation.Name,
                            new ScreenAnimation(currentAnimation.FramesNum,
                                currentAnimation.Looping,
                                isPortrait,
                                currentAnimation.Textures));
                    }
                    else
                    {
                        string[] coordinates = currentAnimation.Origin.Split(',');
                        var origin = new Vector2(Convert.ToSingle(coordinates[0]), Convert.ToSingle(coordinates[1]));
                        animationDictionary.Add(currentAnimation.Name, new ScreenAnimation(
                            currentAnimation.FramesNum,
                            currentAnimation.Looping,
                            isPortrait,
                            currentAnimation.Textures, origin));
                    }
                }
            }

            _sprites.Add(currentSprite.Attribute("Name").Value, new Sprite(animationDictionary));

            return MoveNext();
        }

        public bool MoveNext()
        {
            _position++;
            return (_position < _items.Length);
        }
    }
}
