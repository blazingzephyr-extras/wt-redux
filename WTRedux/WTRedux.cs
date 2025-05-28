using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Runtime.Intrinsics.Arm;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Xna.Framework.Audio;

using Icculus.PhysFS.NET;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Diagnostics;
using Icculus.PhysFS.NET.Internals;
using Spine;
using Microsoft.Xna.Framework.Input;

namespace WorldTour;

internal partial class WTGame : Game
{
    [STAThread]
    public static void Main()
    {
        Environment.SetEnvironmentVariable("FNA_PLATFORM_BACKEND", "SDL3");
        Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", "D3D11");

        WTGame g = new WTGame();
        g.Run();
    }

    Texture2D texture;
    private SpriteBatch batch;

    public WTGame()
    {
        var gr = new GraphicsDeviceManager(this);
        gr.PreferredBackBufferWidth = 2560;
        gr.PreferredBackBufferHeight = 1440;
        gr.IsFullScreen = true;
    }

    Skeleton skeleton;
    AnimationState state;

    protected override void LoadContent()
    {
        batch = new SpriteBatch(GraphicsDevice);

        PhysFS.Init();
        PhysFS.SetSaneConfig("blazingzephyr", "PhysFS.NET", ".zip", false, true);
        PhysFS.Mount(PhysFS.WriteDirectory, false);

        FileSystemObject file = PhysFS.OpenFile("new.txt", FileSystemObjectAccess.Append);
        file.WriteBytes(Encoding.UTF8.GetBytes("Sample text"));
        PhysFS.CloseFile(file);

        FnaTextureLoader loader = new FnaTextureLoader(GraphicsDevice);

        file = PhysFS.OpenFile("celestial-circus.atlas", FileSystemObjectAccess.Read);
        UnmanagedMemoryStream data = file.ReadToStream();
        StreamReader dataReader = new StreamReader(data);
        Atlas atlas = new Atlas(dataReader, "", loader);
        dataReader.Close();
        PhysFS.CloseFile(file);

        SkeletonJson json = new SkeletonJson(atlas);
        json.Scale = 0.15f;

        file = PhysFS.OpenFile("celestial-circus-pro.json", FileSystemObjectAccess.Read);
        data = file.ReadToStream();
        dataReader = new StreamReader(data);
        SkeletonData skeletonData = json.ReadSkeletonData(dataReader);
        dataReader.Close();
        PhysFS.CloseFile(file);

        PhysFS.Deinit();

        skeletonRenderer = new SkeletonRenderer(GraphicsDevice);
        skeletonRenderer.PremultipliedAlpha = false;

        skeleton = new Skeleton(skeletonData);
        AnimationStateData stateData = new AnimationStateData(skeleton.Data);
        state = new AnimationState(stateData);

        skeleton.X = GraphicsDevice.Viewport.Width / 2;
        skeleton.Y = GraphicsDevice.Viewport.Height * 2f / 3f;

        state.SetAnimation(0, "swing", true);
        state.SetAnimation(1, "eyeblink", true);
    }

    protected SkeletonRenderer skeletonRenderer;

    protected override void Draw(GameTime gameTime)
    {
        float deltaTime = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0);

        skeleton.X = Mouse.GetState().X;
        skeleton.Y = Mouse.GetState().Y;

        state.Update(deltaTime);
        skeleton.Update(deltaTime);
        // Note: if you are not directly modifying skeleton.X or .Y, you can apply external
        // movement to physics via the following code:
        // Vector2 lastPosition; // add as a member variable
        // ..
        // Vector2 currentPosition = <current world position>;
        // Vector2 externalPositionDelta = currentPosition - lastPosition;
        // skeleton.PhysicsTranslate(externalPositionDelta.x, externalPositionDelta.y);
        // lastPosition = currentPosition;

        state.Apply(skeleton);
        skeleton.UpdateWorldTransform(Skeleton.Physics.Update);

        GraphicsDevice.Clear(Color.Black);
        ((BasicEffect)skeletonRenderer.Effect).Projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 1, 0);

        skeletonRenderer.Begin();
        skeletonRenderer.Draw(skeleton);
        skeletonRenderer.End();

        base.Draw(gameTime);
    }
}


/*

        // First of all, initialize the library!
        PhysFS.Init();

        // This will set basic values, for example, the writing directory
        // will be set to "AppData/Roaming/blazingzephyr/PhysFS.NET".
        PhysFS.SetSaneConfig("blazingzephyr", "PhysFS.NET", ".zip", false, true);

        // This will add the application's directory as an available search path.
        PhysFS.Mount(PhysFS.BasePath, false);

        // This is probably expected to be in the application's directory.
        // Or could be in the writing dir.
        FileSystemObject file = PhysFS.OpenFile("file.txt", FileSystemObjectAccess.Read);

        // Equivalent to PhysFS.ReadBytes(file) (calls it internally).
        // Use whichever you prefer
        byte[] fileContents = file.ReadBytes();

        // Do whatever you want with the contents.
        Console.WriteLine(fileContents.Length);

        // Equivalent to PhysFS.CloseFile(file).
        file.Dispose();

        // This will open a file for writing.
        // If the file exists, it will be truncated (emptied).
        // if the file doesn't exist, it will be created (presumably in the write dir).
        file = PhysFS.OpenFile("new.txt", FileSystemObjectAccess.Truncate);

        // PhysFS uses UTF8 encoding!
        byte[] content = Encoding.UTF8.GetBytes("Sample text");

        // This will write bytes to a file.
        // You can also use this with FileSystemObjectAccess.Append
        // to instead append contents to a file.
        file.WriteBytes(content);

        // Close the file.
        file.Dispose();

        // You can set it to something meaningless if you don't need it.
        int data = 0;

        // This will list every file found in "/".
        PhysFS.Enumerate("/", EnumerateCallback, ref data);
        // Our callback for enumerating files.
        static EnumerateCallbackResult EnumerateCallback(ref int data,
        string directory, string file)
        {
            // But you can use this for writing data from within too.
            data += 1;

            // You can do whatever you want with the provided data.
            // Open files, for example.
            Console.WriteLine($"{directory}{file}");

            if (data == 10)
            {
                // This will signal that we should stop on the 10th file found.
                return EnumerateCallbackResult.Stop;
            }

            // Signals that we should continue looking for files.
            return EnumerateCallbackResult.Continue;
        }

        // Don't forget to deinitialize PhysFS when you're done!
        PhysFS.Deinit();
 */
