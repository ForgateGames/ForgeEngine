using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;
using System;

namespace ForgeEngine
{
    public class Game1 : Core
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static ImGuiRenderer GuiRenderer;
        bool _toolActive = true;
        System.Numerics.Vector4 _colorV4;

        protected override void Initialize()
        {
            _colorV4 = Color.CornflowerBlue.ToVector4().ToNumerics();
            base.Initialize();

            Scene = Scene.CreateWithDefaultRenderer();

            var imGuiManager = new ImGuiManager();
            Core.RegisterGlobalManager(imGuiManager);
            imGuiManager.RegisterDrawCommand(Teste);

            // toggle ImGui rendering on/off. It starts out enabled.
            imGuiManager.SetEnabled(true);
        }

        private void Teste()
        {
            //ImGui.Begin("Your ImGui Window");
            // your ImGui commands here
            Scene.ClearColor = new Color(_colorV4);
            if (_toolActive)
            {
                ImGui.Begin("My First Tool", ref _toolActive, ImGuiWindowFlags.MenuBar);
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Open..", "Ctrl+O")) { /* Do stuff */ }
                        if (ImGui.MenuItem("Save", "Ctrl+S")) { /* Do stuff */ }
                        if (ImGui.MenuItem("Close", "Ctrl+W")) { _toolActive = false; }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenuBar();
                }

                // Edit a color stored as 4 floats
                ImGui.ColorEdit4("Eduardo", ref _colorV4);

                // Generate samples and plot them
                var samples = new float[100];
                for (var n = 0; n < samples.Length; n++)
                    samples[n] = (float)Math.Sin(n * 0.2f + ImGui.GetTime() * 1.5f);
                ImGui.PlotLines("Samples", ref samples[0], 100);

                // Display contents in a scrolling region
                ImGui.TextColored(new Vector4(1, 1, 0, 1).ToNumerics(), "Important Stuff");
                ImGui.BeginChild("Scrolling", new System.Numerics.Vector2(0));
                for (var n = 0; n < 50; n++)
                    ImGui.Text($"{n:0000}: Some text");
                ImGui.EndChild();
                ImGui.End();
            }

            //ImGui.End();
        }
    }
}
