using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    public class Camera
    {
        private float _angle;

        private bool _dirty;
        private Matrix _inverse = Matrix.Identity;
        private Matrix _matrix = Matrix.Identity;
        private Vector2 _origin = Vector2.Zero;

        public Matrix BackdropMatrix = Matrix.Identity;

        private Vector2 _position = Vector2.Zero;
        private Vector2 _zoom = Vector2.One;
        private bool _lockToPixel = true;

        public Viewport Viewport;

        public Camera()
        {
            Viewport = new Viewport {Width = Engine.Width, Height = Engine.Height};
            UpdateMatrices();
        }

        public Camera(int width, int height)
        {
            Viewport = new Viewport {Width = width, Height = height};
            UpdateMatrices();
        }

        public bool LockToPixel
        {
            get => _lockToPixel;
            set
            {
                _lockToPixel = value;
                _dirty = true;
            }
        }

        public Matrix Matrix
        {
            get
            {
                if (_dirty) UpdateMatrices();

                return _matrix;
            }
        }

        public Matrix Inverse
        {
            get
            {
                if (_dirty) UpdateMatrices();

                return _inverse;
            }
        }

        public Vector2 Position
        {
            get => _position;
            set
            {
                _dirty = true;
                _position = value;
            }
        }

        public Vector2 Origin
        {
            get => _origin;
            set
            {
                _dirty = true;
                _origin = value;
            }
        }

        public float X
        {
            get => _position.X;
            set
            {
                _dirty = true;
                _position.X = value;
            }
        }

        public float Y
        {
            get => _position.Y;
            set
            {
                _dirty = true;
                _position.Y = value;
            }
        }

        public float Zoom
        {
            get => _zoom.X;
            set
            {
                _dirty = true;
                _zoom.X = _zoom.Y = value;
            }
        }

        public float Angle
        {
            get => _angle;
            set
            {
                _dirty = true;
                _angle = value;
            }
        }

        public float Left
        {
            get
            {
                if (_dirty) UpdateMatrices();

                var corner1 = ScreenToCamera(Vector2.Zero);
                var corner2 = ScreenToCamera(new Vector2(Viewport.Width, 0));
                var corner3 = ScreenToCamera(new Vector2(0, Viewport.Height));
                var corner4 = ScreenToCamera(new Vector2(Viewport.Width, Viewport.Height));

                return Mathf.Min(corner1.X, corner2.X, corner3.X, corner4.X);
            }
            set
            {
                if (_dirty) UpdateMatrices();

                X = Vector2.Transform(Vector2.UnitX * value, Matrix).X;
            }
        }

        public float Right
        {
            get
            {
                if (_dirty) UpdateMatrices();

                var corner1 = ScreenToCamera(Vector2.Zero);
                var corner2 = ScreenToCamera(new Vector2(Viewport.Width, 0));
                var corner3 = ScreenToCamera(new Vector2(0, Viewport.Height));
                var corner4 = ScreenToCamera(new Vector2(Viewport.Width, Viewport.Height));

                return Mathf.Max(corner1.X, corner2.X, corner3.X, corner4.X);
            }
            set => throw new NotImplementedException();
        }

        public float Top
        {
            get
            {
                if (_dirty) UpdateMatrices();

                var corner1 = ScreenToCamera(Vector2.Zero);
                var corner2 = ScreenToCamera(new Vector2(Viewport.Width, 0));
                var corner3 = ScreenToCamera(new Vector2(0, Viewport.Height));
                var corner4 = ScreenToCamera(new Vector2(Viewport.Width, Viewport.Height));
                return Mathf.Min(corner1.Y, corner2.Y, corner3.Y, corner4.Y);
            }
            set
            {
                if (_dirty) UpdateMatrices();

                Y = Vector2.Transform(Vector2.UnitY * value, Matrix).Y;
            }
        }

        public float Bottom
        {
            get
            {
                if (_dirty) UpdateMatrices();

                var corner1 = ScreenToCamera(Vector2.Zero);
                var corner2 = ScreenToCamera(new Vector2(Viewport.Width, 0));
                var corner3 = ScreenToCamera(new Vector2(0, Viewport.Height));
                var corner4 = ScreenToCamera(new Vector2(Viewport.Width, Viewport.Height));
                return Mathf.Max(corner1.Y, corner2.Y, corner3.Y, corner4.Y);
            }
            set => throw new NotImplementedException();
        }

        public Rectangle Bounds
        {
            get
            {
                var corner1 = ScreenToCamera(Vector2.Zero);
                var corner2 = ScreenToCamera(new Vector2(Viewport.Width, 0));
                var corner3 = ScreenToCamera(new Vector2(0, Viewport.Height));
                var corner4 = ScreenToCamera(new Vector2(Viewport.Width, Viewport.Height));

                var cameraLeft = Mathf.Min(corner1.X, corner2.X, corner3.X, corner4.X);
                var cameraRight = Mathf.Max(corner1.X, corner2.X, corner3.X, corner4.X);
                var cameraTop = Mathf.Min(corner1.Y, corner2.Y, corner3.Y, corner4.Y);
                var cameraBottom = Mathf.Max(corner1.Y, corner2.Y, corner3.Y, corner4.Y);

                return new Rectangle((int) cameraLeft, (int) cameraTop, (int) (cameraRight - cameraLeft),
                    (int) (cameraBottom - cameraTop));
            }
        }

        public override string ToString()
        {
            return "Camera:\n\tViewport: { " + Viewport.X + ", " + Viewport.Y +
                   ", " +
                   Viewport.Width + ", " + Viewport.Height +
                   " }\n\tPosition: { " + _position.X + ", " + _position.Y +
                   " }\n\tOrigin: { " + _origin.X + ", " + _origin.Y +
                   " }\n\tZoom: { " + _zoom.X + ", " + _zoom.Y +
                   " }\n\tAngle: " + _angle;
        }

        private void UpdateMatrices()
        {
            if (_lockToPixel)
            {
                _matrix = Matrix.Identity *
                          Matrix.CreateTranslation(
                              new Vector3(-new Vector2(Mathf.FloorToInt(_position.X), Mathf.FloorToInt(_position.Y)), 0)) *
                          Matrix.CreateRotationZ(_angle) *
                          Matrix.CreateScale(new Vector3(_zoom, 1)) *
                          Matrix.CreateTranslation(
                              new Vector3(new Vector2(Mathf.FloorToInt(_origin.X), Mathf.FloorToInt(_origin.Y)), 0));
                BackdropMatrix = Matrix.Identity                           *
                                 Matrix.CreateTranslation(
                                     new Vector3(-Mathf.FloorToInt(_origin.X), -Mathf.FloorToInt(_origin.Y), 0)) *
                                 Matrix.CreateRotationZ(_angle)            *
                                 Matrix.CreateScale(new Vector3(_zoom, 1)) *
                                 Matrix.CreateTranslation(
                                     new Vector3(Mathf.FloorToInt(_origin.X), Mathf.FloorToInt(_origin.Y), 0));
            }
            else
            {
                _matrix = Matrix.Identity *
                          Matrix.CreateTranslation(
                              new Vector3(-new Vector2(_position.X, _position.Y), 0)) *
                          Matrix.CreateRotationZ(_angle) *
                          Matrix.CreateScale(new Vector3(_zoom, 1)) *
                          Matrix.CreateTranslation(
                              new Vector3(new Vector2(_origin.X, _origin.Y), 0));
                BackdropMatrix = Matrix.Identity                           *
                                 Matrix.CreateTranslation(
                                     new Vector3(-_origin.X, -_origin.Y, 0)) *
                                 Matrix.CreateRotationZ(_angle)                                                  *
                                 Matrix.CreateScale(new Vector3(_zoom, 1))                                       *
                                 Matrix.CreateTranslation(
                                     new Vector3(_origin.X, _origin.Y, 0));
            }

            _inverse = Matrix.Invert(_matrix);
            _dirty = false;
        }

        public void CopyFrom(Camera other)
        {
            _position = other._position;
            _origin = other._origin;
            _angle = other._angle;
            _zoom = other._zoom;
            _dirty = true;
        }

        public void CenterOrigin()
        {
            _origin = new Vector2((float) Viewport.Width / 2, (float) Viewport.Height / 2);
            _dirty = true;
        }

        public void RoundPosition()
        {
            _position.X = Mathf.Round(_position.X);
            _position.Y = Mathf.Round(_position.Y);
            _dirty = true;
        }

        public Vector2 ScreenToCamera(Vector2 position)
        {
            return Vector2.Transform(position, Inverse);
        }

        public Vector2 CameraToScreen(Vector2 position)
        {
            return Vector2.Transform(position, Matrix);
        }

        public void Approach(Vector2 position, float ease)
        {
            Position += (position - Position) * ease;
        }

        public void Approach(Vector2 position, float ease, float maxDistance)
        {
            var move = (position - Position) * ease;
            if (move.Length() > maxDistance)
                Position += Vector2.Normalize(move) * maxDistance;
            else
                Position += move;
        }
    }
}
