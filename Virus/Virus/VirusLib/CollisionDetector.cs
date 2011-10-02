using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VirusLib
{
	public enum BorderType
	{
		top,
		bottomPortrait,
		bottomLandscape,
		left,
		rightPortrait,
		rightLandscape
	}

	public static class CollisionDetector
	{
		public static bool CollisionWithBorder(Vector2 position, float angle, Vector2 speed, Shape shape, BorderType borderType)
		{
			switch (shape.GetShapeType())
			{
				case ShapeType.circular:

					float radius = shape.GetShapeSize().X;

					switch (borderType)
					{
						case BorderType.top:
							return (speed.Y < 0 && (position.Y - radius <= 0));

						case BorderType.bottomPortrait:
							return (speed.Y > 0 && (position.Y + radius >= 800));

						case BorderType.bottomLandscape:
							return (speed.Y > 0 && (position.Y + radius >= 480));

						case BorderType.left:
							return (speed.X < 0 && (position.X - radius <= 0));

						case BorderType.rightPortrait:
							return (speed.X > 0 && (position.X + radius >= 480));

						case BorderType.rightLandscape:
							return (speed.X > 0 && (position.X + radius >= 800));

						default:
							throw new ArgumentException("Error in CollisionWithBorder", "borderType");
					}

				case ShapeType.rectangular:

                    throw new Exception("not already implemented!");

				default:
					throw new ArgumentException("Error in CollisionWithBorder", "shapeType");
			}
		}

        public static bool CircularBodyGeneralBodyCollision(Vector2 position1, float radius1, Vector2 position2, float angle2, Shape shape)
        {
            switch (shape.GetShapeType())
            {
                case ShapeType.circular:
                    float radius2 = shape.GetShapeSize().X;
                    return CirularBodyCollision(position1, radius1, position2, radius2);

                case ShapeType.rectangular:
                    throw new Exception("not already implemented!");


                default:
                    throw new ArgumentException("Error in CircularBodyGeneralBodyCollision", "shapeType");
            }
        }

        public static bool CirularBodyCollision(Vector2 position1, float radius1, Vector2 position2, float radius2)
        {
            return (Vector2.Distance(position1, position2) < radius1 + radius2);
        }
	}
}
