using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SME
{
    public static class Geometrie
    {
        public enum Layout
        {
            Center,
            Corner
        }

        private static Texture2D pixel;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            CreateThePixel(graphicsDevice);
        }

        private static void CreateThePixel(GraphicsDevice graphicsDevice)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

        public static void DrawPoints(SpriteBatch spriteBatch, Vector2 position, List<Vector2> points, Color color, float thickness)
        {
            if (points.Count < 2)
                return;

            for (int i = 1; i < points.Count; i++)
            {
                DrawLine(spriteBatch, points[i - 1] + position, points[i] + position, color, thickness, 0f);
            }
        }

        public static void DrawVector(this SpriteBatch spriteBatch, in Vector2 v, in Vector2 position, in Color color)
        {
            spriteBatch.DrawLine(position, position + v, color);
            spriteBatch.DrawCircle(position + v, 5, color);
        }

        #region DrawPixel
        /// <summary>
        /// Draw the pixel at the position
        /// </summary>
        public static void DrawPixel(this SpriteBatch spriteBatch, in Vector2 position, in Color color, float layerDepth = 0f)
        {
            spriteBatch.Draw(pixel, position, null, color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, layerDepth);
        }
        #endregion

        #region DrawLine

        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float layerDepth = 0f)
        {
            DrawLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, 1.0f, layerDepth);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float thickness, float layerDepth = 0f)
        {
            DrawLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, thickness, layerDepth);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float layerDepth = 0f)
        {
            DrawLine(spriteBatch, point1, point2, color, 1.0f, layerDepth);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness, float layerDepth = 0f)
        {
            // calculate the distance between the two vectors
            float distance = Vector2.Distance(point1, point2);

            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(spriteBatch, point1, distance, angle, color, thickness, layerDepth);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float layerDepth = 0f)
        {
            DrawLine(spriteBatch, point, length, angle, color, 1.0f, layerDepth);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness, float layerDepth = 0f)
        {
            // stretch the pixel between the two vectors
            spriteBatch.Draw(pixel, point, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, layerDepth);
        }        

        #endregion

        #region DrawDroite

        public static void DrawDroite(this SpriteBatch spriteBatch, in Vector2 A, in Vector2 B, in Color color)
        {
            if (Math.Abs(A.X - B.X) < 1f)
            {
                spriteBatch.DrawLine(new Vector2((A.X + B.X) / 2f, -10f), new Vector2((A.X + B.X) / 2f, Screen.height + 10f), color);
            }
            else
            {
                float a = (B.Y - A.Y) / (B.X - A.X);
                float b = A.Y - a * A.X;
                spriteBatch.DrawLine(new Vector2(-10f, a * -10f + b), new Vector2(Screen.width + 10f, a * (Screen.width + 10f) + b), color);
            }
        }
        public static void DrawDroite(this SpriteBatch spriteBatch, in Vector2 A, in Vector2 B)
        {
            spriteBatch.DrawDroite(A, B, Color.Black);
        }

        #endregion

        #region DrawCircle
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, int radius, int sides, Color color, float thickness)
        {
            new Circle(center, radius, thickness, sides).Draw(spriteBatch, color);
        }
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, int radius, Color color, float thickness)
        {
            new Circle(center, radius, thickness).Draw(spriteBatch, color);
        }
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, int radius, Color color)
        {
            new Circle(center, radius, 1f).Draw(spriteBatch, color);
        }
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, int radius)
        {
            new Circle(center, radius, 1f).Draw(spriteBatch, Color.Black);
        }
        public static void DrawCircle(this SpriteBatch spriteBatch, int x, int y, int radius, int sides, Color color, float thickness)
        {
            new Circle(new Vector2(x, y), radius, thickness, sides).Draw(spriteBatch, color);
        }
        public static void DrawCircle(this SpriteBatch spriteBatch, int x, int y, int radius, Color color, float thickness)
        {
            new Circle(new Vector2(x, y), radius, thickness).Draw(spriteBatch, color);
        }
        public static void DrawCircle(this SpriteBatch spriteBatch, int x, int y, int radius, Color color)
        {
            new Circle(new Vector2(x, y), radius, 1f).Draw(spriteBatch, color);
        }
        public static void DrawCircle(this SpriteBatch spriteBatch, int x, int y, int radius)
        {
            new Circle(new Vector2(x, y), radius, 1f).Draw(spriteBatch);
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, int radius, int sides, Color color, float thickness)
        {
            new Circle(new Vector2(x, y), radius, thickness, sides).Draw(spriteBatch, color);
        }
        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, int radius, Color color, float thickness)
        {
            new Circle(new Vector2(x, y), radius, thickness).Draw(spriteBatch, color);
        }
        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, int radius, Color color)
        {
            new Circle(new Vector2(x, y), radius, 1f).Draw(spriteBatch, color);
        }
        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, int radius)
        {
            new Circle(new Vector2(x, y), radius, 1f).Draw(spriteBatch);
        }

        #endregion

        #region DrawRectangle
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, Color colorCenter, int thickness = 1, float layerDepth = 0f)
        {
            spriteBatch.DrawRectangleFill(new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color, layerDepth);
            spriteBatch.DrawRectangleFill(new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color, layerDepth);
            spriteBatch.DrawRectangleFill(new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color, layerDepth);
            spriteBatch.DrawRectangleFill(new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color, layerDepth);
            spriteBatch.DrawRectangleFill(new Rectangle(rectangle.X + thickness, rectangle.Y + thickness, rectangle.Width - 2 * thickness, rectangle.Height - 2 * thickness), colorCenter, layerDepth);
        }
        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 position, int width, int height, Color color, Color colorCenter, int thickness = 1, float layerDepth = 0f)
        {
            spriteBatch.DrawRectangleFill(position, new Vector2(width, thickness), color, layerDepth);
            spriteBatch.DrawRectangleFill(new Vector2(position.X, position.Y + height - thickness), new Vector2(width, thickness), color, layerDepth);
            spriteBatch.DrawRectangleFill(new Vector2(position.X + width - thickness, position.Y), new Vector2(thickness, height), color, layerDepth);
            spriteBatch.DrawRectangleFill(position, new Vector2(thickness, height), color, layerDepth);
            spriteBatch.DrawRectangleFill(new Vector2(position.X + thickness, position.Y + thickness), new Vector2(width - 2 * thickness, height - 2 * thickness), colorCenter, layerDepth);
        }
        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color, Color colorCenter, int thickness = 1, float layerDepth = 0f)
        {
            spriteBatch.DrawRectangleFill(position, new Vector2(size.X, thickness), color, layerDepth);
            spriteBatch.DrawRectangleFill(new Vector2(position.X, position.Y + size.Y - thickness), new Vector2(size.X, thickness), color, layerDepth);
            spriteBatch.DrawRectangleFill(new Vector2(position.X + size.X - thickness, position.Y), new Vector2(thickness, size.Y), color, layerDepth);
            spriteBatch.DrawRectangleFill(position, new Vector2(thickness, size.Y), color, layerDepth);
            spriteBatch.DrawRectangleFill(new Vector2(position.X + thickness, position.Y + thickness), new Vector2(size.X - 2 * thickness, size.Y - 2 * thickness), colorCenter, layerDepth);
        }
        public static void DrawRectangleFill(this SpriteBatch spriteBatch, Vector2 position, int width, int height, Color color, float layerDepth = 0f)
        {
            spriteBatch.Draw(pixel, position, null, color, 0f, Vector2.Zero, new Vector2(width, height), SpriteEffects.None, layerDepth);
        }
        public static void DrawRectangleFill(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color, float layerDepth = 0f)
        {
            spriteBatch.Draw(pixel, position, null, color, 0, Vector2.Zero, size, SpriteEffects.None, layerDepth);
        }
        public static void DrawRectangleFill(this SpriteBatch spriteBatch, Rectangle rec, Color color, float layerDepth = 0f)
        {
            spriteBatch.Draw(pixel, rec, null, color, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        }

        #endregion

        #region DrawString in rectangle
        /// <summary>
        /// to change color in the texte put /C:N where N is th index of the color in colorList.
        /// put $ tu jump on line.
        /// </summary>
        public static void DrawString(this SpriteBatch spriteBatch, Rectangle rec, SpriteFont font, string text, List<Color> colorsText, Color colorRec, Color colorCenter, Vector2 scaleText, float rotationText = 0f, int thinkness = 1, Layout layout = Layout.Corner, float layerDepth = 0f)
        {
            spriteBatch.DrawRectangle(rec, colorRec, colorCenter, thinkness, layerDepth);

            if(layout == Layout.Corner)//ok
            {
                Vector2 posText = new Vector2(rec.X + rec.Width * 0.05f, rec.Y + rec.Height * 0.05f);
                int indexChar = 0;
                int indexColor = 0;
                while (indexChar < text.Length)
                {
                    string mot = "";
                    string letter;
                    bool sautDeLigne = false;
                    do
                    {
                        letter = text.Substring(indexChar, 1);
                        if (letter == "/")
                        {
                            switch (text.Substring(indexChar + 1, 2))
                            {
                                case "C:":
                                    int newIndexColor = Useful.ConvertStringToInt(text.Substring(indexChar + 3, 1));
                                    if (newIndexColor >= 0 && newIndexColor < colorsText.Count)
                                        indexColor = newIndexColor;
                                    indexChar += 3;
                                    break;
                                default:
                                    mot += letter;
                                    break;
                            }
                        }
                        else if (letter == "$")
                        {
                            sautDeLigne = true;
                            letter = " ";
                            mot += letter;
                        }
                        else
                        {
                            mot += letter;
                        }
                        indexChar++;
                    } while (letter != " " && letter != "," && letter != "." && letter != "!" && letter != ";" && indexChar < text.Length);

                    Vector2 wordSize = font.MeasureString(mot) * scaleText;
                    if (posText.X + wordSize.X > rec.X + 0.95f * rec.Width)
                    {
                        posText = new Vector2(rec.X + rec.Width * 0.05f, posText.Y + wordSize.Y);
                    }
                    spriteBatch.DrawString(font, mot, posText, colorsText[indexColor], rotationText, Vector2.Zero, scaleText, SpriteEffects.None, layerDepth);

                    posText = sautDeLigne ? new Vector2(rec.X + rec.Width * 0.05f, posText.Y + wordSize.Y) : new Vector2(posText.X + wordSize.X, posText.Y);
                }
            }
            else // center, presque OK (nbLine a revoir et affiché un mot cour)
            {
                Vector2 textSize = font.MeasureString(text) * scaleText;
                int nbSautDeLigne = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '$')
                        nbSautDeLigne++;
                }
                float nbline1 = textSize.X / (0.9f * rec.Width);
                if (nbline1 - (int)nbline1 > 0.1f)
                    nbline1++;
                int nbLine = nbSautDeLigne + (int)nbline1;
                float offsetY = (rec.Height - textSize.Y * nbLine) / 2f;

                Vector2 posText = new Vector2(0, rec.Y + offsetY);
                int indexChar = 0;
                int indexColor = 0;
                float sizeSentenceSurplus = 0f;

                while (indexChar < text.Length)
                {
                    string sentence = "";
                    string letter;
                    sizeSentenceSurplus = 0f;
                    bool sentenceFinish = false;

                    while (indexChar < text.Length)
                    {
                        letter = "";
                        string word = "";
                        do
                        {
                            letter = text.Substring(indexChar, 1);
                            if (letter == "/")
                            {
                                switch (text.Substring(indexChar + 1, 2))
                                {
                                    case "C:":
                                        string groupe = text.Substring(indexChar, 4);
                                        indexChar += 3;
                                        sizeSentenceSurplus += (font.MeasureString(groupe) * scaleText).X;
                                        word += groupe;
                                        break;
                                    default:
                                        word += letter;
                                        break;
                                }
                            }
                            else if (letter == "$")
                            {
                                indexChar++;
                                sentenceFinish = true;
                                sentence += word;
                                break;
                            }
                            else
                            {
                                word += letter;
                            }
                            indexChar++;
                        } while (letter != " " && letter != "," && letter != "." && letter != "!" && letter != ";" && indexChar < text.Length);

                        if (sentenceFinish)
                            break;

                        if (font.MeasureString(sentence + word).X - sizeSentenceSurplus  < 0.9f * rec.Width)
                        {
                            sentence += word;
                        }
                        else
                        {
                            if(sentence != "")//cas normal
                            {
                                indexChar -= word.Length;
                                break;
                            }
                            else // un seul mot trop long
                            {
                                string wordCut = "";
                                float sizeWordSurplus = 0f;
                                if(word.Substring(0, 1) == "/")
                                {
                                    switch(word.Substring(1, 2))
                                    {
                                        case "C:":
                                            string groupe = text.Substring(0, 4);
                                            sizeWordSurplus += (font.MeasureString(groupe) * scaleText).X;                                            
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                for (int i = 0; i < word.Length; i++)
                                {
                                    string letter2 = word.Substring(i, 1);

                                    if ((font.MeasureString(wordCut + letter2) * scaleText).X - sizeWordSurplus < 0.9f * rec.Width)
                                    {
                                        wordCut += letter2;
                                    }
                                    else
                                    {
                                        indexChar -= (word.Length - i);
                                        sentence = wordCut;
                                    }
                                }
                            }
                        }
                    }

                    //affichage
                    Vector2 sentenceSize = font.MeasureString(sentence) * scaleText - new Vector2(sizeSentenceSurplus, 0f);
                    posText = new Vector2(rec.X + (rec.Width - sentenceSize.X)/2f, posText.Y);                    
                    
                    for (int i = 0; i < sentence.Length; i++)
                    {
                        string charShow = "";
                        letter = sentence.Substring(i, 1);
                        if (letter == "/")
                        {
                            switch (sentence.Substring(i + 1, 2))
                            {
                                case "C:":
                                    int newIndexColor = Useful.ConvertStringToInt(sentence.Substring(i + 3, 1));
                                    if (newIndexColor >= 0 && newIndexColor < colorsText.Count)
                                        indexColor = newIndexColor;
                                    i += 3;
                                    break;
                                default:
                                    charShow = letter;
                                    break;
                            }
                        }
                        else
                        {
                            charShow = letter;
                        }
                        spriteBatch.DrawString(font, charShow, posText, colorsText[indexColor], rotationText, Vector2.Zero, scaleText, SpriteEffects.None, layerDepth - 0.000001f);
                        posText = new Vector2(posText.X + font.MeasureString(charShow).X, posText.Y);
                    }
                    if(sentenceSize.Y != 0)
                        posText = new Vector2(0, posText.Y + sentenceSize.Y);
                    else
                        posText = new Vector2(0, posText.Y + font.MeasureString("l").Y);
                }
            }
        }

        public static void DrawString(this SpriteBatch spriteBatch, Rectangle rec, SpriteFont font, string text, List<Color> colorsText, Color colorRec, Color colorCenter, int thinkness = 1, Layout layout = Layout.Corner, float layerDepth = 0f)
        {
            spriteBatch.DrawString(rec, font, text, colorsText, colorRec, colorCenter, Vector2.One, 0f, thinkness, layout, layerDepth);
        }        
        public static void DrawString(this SpriteBatch spriteBatch, Rectangle rec, SpriteFont font, string text, Color colorText, Color colorRec, Color colorCenter, Vector2 scaleText, float rotationText = 0f, int thinkness = 1, Layout layout = Layout.Corner, float layerDepth = 0f)
        {
            spriteBatch.DrawString(rec, font, text, new List<Color> { colorText }, colorRec, colorCenter, scaleText, rotationText, thinkness, layout, layerDepth);
        }
        public static void DrawString(this SpriteBatch spriteBatch, Rectangle rec, SpriteFont font, string text, Color colorText, Color colorRec, Color colorCenter, int thinkness = 1, Layout layout = Layout.Corner, float layerDepth = 0f)
        {
            spriteBatch.DrawString(rec, font, text, new List<Color> { colorText }, colorRec, colorCenter, Vector2.One, 0f, thinkness, layout, layerDepth);
        }
        /// <summary>
        /// N'affiche pas le rectangle
        /// </summary>
        public static void DrawString(this SpriteBatch spriteBatch, Rectangle rec, SpriteFont font, string text, List<Color> colorsText, Vector2 scaleText, float textRotation, Layout layout = Layout.Corner, float layerDepth = 0f)
        {
            spriteBatch.DrawString(rec, font, text, colorsText, Color.Transparent, Color.Transparent, scaleText, textRotation, 0, layout, layerDepth);
        }
        /// <summary>
        /// N'affiche pas le rectangle
        /// </summary>
        public static void DrawString(this SpriteBatch spriteBatch, Rectangle rec, SpriteFont font, string text, List<Color> colorsText, Layout layout = Layout.Corner, float layerDepth = 0f)
        {
            spriteBatch.DrawString(rec, font, text, colorsText, Color.Transparent, Color.Transparent, Vector2.One, 0f, 0, layout, layerDepth);
        }
        /// <summary>
        /// N'affiche pas le rectangle
        /// </summary>
        public static void DrawString(this SpriteBatch spriteBatch, Rectangle rec, SpriteFont font, string text, Color colorText, Vector2 scaleText, float textRotation, Layout layout = Layout.Corner, float layerDepth = 0f)
        {
            spriteBatch.DrawString(rec, font, text, new List<Color> { colorText }, Color.Transparent, Color.Transparent, scaleText, textRotation, 0, layout, layerDepth);
        }
        /// <summary>
        /// N'affiche pas le rectangle
        /// </summary>
        public static void DrawString(this SpriteBatch spriteBatch, Rectangle rec, SpriteFont font, string text, Color colorText, Layout layout = Layout.Corner, float layerDepth = 0f)
        {
            spriteBatch.DrawString(rec, font, text, new List<Color> { colorText }, Color.Transparent, Color.Transparent, Vector2.One, 0f, 0, layout, layerDepth);
        }
        #endregion

        #region DrawString in Circle
        public static void DrawString(this SpriteBatch spriteBatch, Circle circle, SpriteFont font, string text, Color colorText, float layerDeep)
        {
            Vector2 textSize = font.MeasureString(text);
            if (textSize.X < 2f * circle.radius && textSize.Y < 2f * circle.radius)
            {
                Vector2 offsetCenter = textSize / 2f;
                spriteBatch.DrawString(font, text, circle.center - offsetCenter, colorText, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDeep);
                circle.Draw(spriteBatch);
            }
            else
            {
                Debug.WriteLine("Le text : " + text + " ne rentre pas dans le cercle : " + circle.ToString());
            }
        }
        public static void DrawString(this SpriteBatch spriteBatch, Circle circle, SpriteFont font, string text, Color colorText, float rotation, Vector2 scale, float layerDeep)
        {
            Vector2 textSize = font.MeasureString(text) * scale;
            if (textSize.X < 2f * circle.radius && textSize.Y < 2f * circle.radius)
            {
                Vector2 offsetCenter = textSize / 2f;
                spriteBatch.DrawString(font, text, circle.center - offsetCenter, colorText, rotation, Vector2.Zero, scale, SpriteEffects.None, layerDeep);
                circle.Draw(spriteBatch);
            }
            else
            {
                Debug.WriteLine("Le text : " + text + " ne rentre pas dans le cercle : " + circle.ToString());
            }
        }
        #endregion
    }
}
