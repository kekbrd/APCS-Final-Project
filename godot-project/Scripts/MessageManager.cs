using Godot;
using Godot.Collections;
using System.Threading.Tasks;
using System;
using System.ComponentModel;

namespace Game.UI; // Almost everything in here is from tutorial: https://www.youtube.com/watch?v=Mrmu_06GYcs&list=PLdSnLYEzOTtqegR6BJAooonhOvg4Am8d_&index=7
public partial class MessageManager : CanvasLayer
{
	public static MessageManager Instance {get; private set;} // makes private Instance and makes accessor method
	
	[ExportCategory("Components")] 
	[Export]
	public NinePatchRect Box; // [Export] gets a field in godot inspector where you can assign the reference

	[Export]
	public RichTextLabel Label;

	[ExportCategory("Variables")]
	[Export]
	public bool textIsScrolling = false;
	
	[Export]
	public int delay = 15; // delay in ms between printing each character

	[Export]
	public Array<string> messages;

	public override void _Ready()
	{
		Instance = this;
	}

	public static void PlayText(params string[] payload) // 
	{
		if (IsReading() || payload.Length == 0) 
		{
			return;
		}

		Instance.messages = [.. payload];
		ScrollText();
	}

    public override void _Input(InputEvent @event) // I did this method myself
	{
		if (@event.IsActionReleased("dismiss_message"))
		{
			if (textIsScrolling)
			{
				delay = 0;
			}
			else if (!textIsScrolling)
			{
				ScrollText();
				delay = 15;
			}
		}
	}

	public static async void ScrollText() // puts message box on screen, if there are none in queue it removes it. Removes string from queue after putting it up.
	{
		if (!IsReading())
		{
			Instance.Box.Visible = true;
		}
		if (Instance.messages.Count == 0)
		{
			Instance.Box.Visible = false;
			return;
		}

		Instance.textIsScrolling = true;
		Instance.Label.Text = "";

		foreach (char letter in Instance.messages[0])
		{
			Instance.Label.Text += letter;
			await Task.Delay(Instance.delay);
		}

		Instance.messages.RemoveAt(0);
		Instance.textIsScrolling = false;
	}

	public static bool IsReading()
	{
		return Instance.Box.Visible;
	}

	public static bool Scrolling()
	{
		return Instance.textIsScrolling;
	}

	public static Array<string> GetMessages()
	{
		return Instance.messages;
	}
}
