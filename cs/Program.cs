using System.Numerics;
using Raylib_cs;

namespace SneakySnake;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        int screenWidth = 800;
        int screenHeight = 600;
        Raylib.InitWindow(screenWidth, screenHeight, "Sneaky Snake");

        Font font = Raylib.GetFontDefault();

        while (!Raylib.WindowShouldClose())
        {
            Vector2 textSize = Raylib.MeasureTextEx(font, "Welcome to Sneaky Snake!", 20, 1);
            Vector2 textPosition = new Vector2((screenWidth - textSize.X) / 2, (screenHeight - textSize.Y) / 2);

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.SkyBlue);
            Raylib.DrawTextEx(font, "Welcome to Sneaky Snake!", textPosition, 20, 1, Color.Black);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}