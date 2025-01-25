
namespace CoreVisualizer.Interfaces
{
    public interface IRenderControl
    {
        void DoRender();
        void AlignCamera(ViewPlane plane);
        void LoadModel(string path);
    }
}
