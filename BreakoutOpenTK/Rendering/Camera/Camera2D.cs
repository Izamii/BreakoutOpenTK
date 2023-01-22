using OpenTK.Mathematics;

namespace BreakoutOpenTK.Rendering.Camera
{
    public class Camera2D
    {
        private Vector2 _focusPoint, _windowSize;

        public Vector2 ViewMatrix
        {
            get => _focusPoint;
            set => _focusPoint = value;
        }
        
        public Vector2 WindowSize
        {
            get => _windowSize;
            set => _windowSize = value;
        }
        
        public float Zoom { get; set; } = 1f;
        
        public Camera2D(Vector2 focusPoint, Vector2 windowSize)
        {
            _focusPoint = focusPoint;
            _windowSize = windowSize;
        }
        
        public Camera2D(Vector2 focusPoint, Vector2 windowSize, float zoom)
        {
            _focusPoint = focusPoint;
            _windowSize = windowSize;
            Zoom = zoom;
        }
        public Matrix4 GetProjectionMatrix()
        {
            float left = _focusPoint.X - _windowSize.X / 2;
            float right = _focusPoint.X + _windowSize.X / 2; 
            float bottom = _focusPoint.Y + _windowSize.Y / 2;
            float top = _focusPoint.Y - _windowSize.Y / 2; 
            return Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, -1, 1) * Matrix4.CreateScale(Zoom);
        }
    }
}