namespace osuTK.Graphics
{
    public static class GLBindingsExtensions
    {
        public static void LoadEntryPoints(this GLBindings bindings) => new GraphicsBindingsBase(bindings).LoadEntryPoints();
    }
}