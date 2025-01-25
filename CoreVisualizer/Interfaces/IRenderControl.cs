
namespace CoreVisualizer.Interfaces
{
    public interface IRenderControl
    {
        void DoRender();
        void AlignCamera(ViewPlane plane);
        void SetAntialiasing(bool isEnable);
        void LoadModel(string path);
    }
}
