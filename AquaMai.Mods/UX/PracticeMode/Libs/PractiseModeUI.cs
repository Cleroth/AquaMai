using System;
using AquaMai.Mods.Fix;
using AquaMai.Core.Helpers;
using AquaMai.Core.Resources;
using Manager;
using UnityEngine;

namespace AquaMai.Mods.UX.PracticeMode.Libs;

public class PracticeModeUI : MonoBehaviour
{
    private static float windowTop => Screen.height - GuiSizes.PlayerWidth + GuiSizes.PlayerWidth * .22f;
    private static float controlHeight => GuiSizes.PlayerWidth * .13f;
    private static float sideButtonWidth => GuiSizes.PlayerWidth * .1f;
    private static float centerButtonWidth => GuiSizes.PlayerWidth * .28f;
    private static int fontSize => (int)(GuiSizes.PlayerWidth * .02f);
    public static bool settingsMode = false;

	private static Rect GetButtonRect(int pos, int row)
    {
        float x;
        float width;
        switch (pos)
        {
            case 0:
                x = GuiSizes.PlayerCenter - centerButtonWidth / 2 - sideButtonWidth - GuiSizes.Margin;
                width = sideButtonWidth;
                break;
            case 1:
                x = GuiSizes.PlayerCenter - centerButtonWidth / 2;
                width = centerButtonWidth;
                break;
            case 2:
                x = GuiSizes.PlayerCenter + centerButtonWidth / 2 + GuiSizes.Margin;
                width = sideButtonWidth;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
        }

        return new Rect(x, windowTop + (GuiSizes.Margin + controlHeight) * row + GuiSizes.Margin, width, controlHeight);
    }

    public void OnGUI()
    {
        var labelStyle = GUI.skin.GetStyle("label");
        labelStyle.fontSize = fontSize;
        labelStyle.alignment = TextAnchor.MiddleCenter;

        var buttonStyle = GUI.skin.GetStyle("button");
        buttonStyle.fontSize = fontSize;

        if (settingsMode)
        {
            GUI.Button(GetButtonRect(0, 1), " - ");
            GUI.Label(GetButtonRect(1, 1), $"Off.A: {GameSettings.JudgeAdjust.a}");
            GUI.Button(GetButtonRect(2, 1), " + ");
            GUI.Button(GetButtonRect(0, 2), " - ");
            GUI.Label(GetButtonRect(1, 2), $"Off.B: {GameSettings.JudgeAdjust.b}");
            GUI.Button(GetButtonRect(2, 2), " + ");
        }
        else
        {
            GUI.Box(new Rect(
                GuiSizes.PlayerCenter - centerButtonWidth / 2 - sideButtonWidth - GuiSizes.Margin * 2,
                windowTop,
                centerButtonWidth + sideButtonWidth * 2 + GuiSizes.Margin * 4,
                controlHeight * 4 + GuiSizes.Margin * 5
            ), "");

            GUI.Button(GetButtonRect(0, 0), Locale.SeekBackward);
            GUI.Button(GetButtonRect(1, 0), Locale.Pause);
            GUI.Button(GetButtonRect(2, 0), Locale.SeekForward);

            if (PracticeMode.repeatStart == -1)
            {
                GUI.Button(GetButtonRect(0, 1), Locale.MarkRepeatStart);
                GUI.Label(GetButtonRect(1, 1), Locale.RepeatNotSet);
            }
            else if (PracticeMode.repeatEnd == -1)
            {
                GUI.Button(GetButtonRect(0, 1), Locale.MarkRepeatEnd);
                GUI.Label(GetButtonRect(1, 1), Locale.RepeatStartSet);
                GUI.Button(GetButtonRect(2, 1), Locale.RepeatReset);
            }
            else
            {
                GUI.Label(GetButtonRect(1, 1), Locale.RepeatStartEndSet);
                GUI.Button(GetButtonRect(2, 1), Locale.RepeatReset);
            }

            GUI.Button(GetButtonRect(0, 2), Locale.SpeedDown);
            GUI.Label(GetButtonRect(1, 2), $"{Locale.Speed} {PracticeMode.speed * 100:000}%");
            GUI.Button(GetButtonRect(2, 2), Locale.SpeedUp);
            GUI.Button(GetButtonRect(1, 3), Locale.SpeedReset);

            GUI.Label(GetButtonRect(0, 3), $"{TimeSpan.FromMilliseconds(PracticeMode.CurrentPlayMsec):mm\\:ss\\.fff}\n{TimeSpan.FromMilliseconds(NotesManager.Instance().getPlayFinalMsec()):mm\\:ss\\.fff}");
            GUI.Button(GetButtonRect(2, 3), $"{Locale.KeepSpeed}\n{(PracticeMode.keepNoteSpeed ? "ON" : "OFF")}");
        }
    }
    public void Update()
    {
        bool cancel = InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.A1) ||
            InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.A2) ||
            InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.A3) ||
            InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.A4) ||
            InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.A5) ||
            InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.A6) ||
            InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.A7) ||
            InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.A8) ||
                InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.D1) ||
                InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.D2) ||
                InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.D3) ||
                InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.D4) ||
                InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.D5) ||
                InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.D6) ||
                InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.D7) ||
                InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.D8);

        if (settingsMode)
        {
            if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B7))
            {
                GameSettings.JudgeAdjust.a -= 0.1f;
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B2))
            {
                GameSettings.JudgeAdjust.a += 0.1f;
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B2))
            {
                // Offset display. maybe don't reset to avoid accidental presses
                // GameSettings.JudgeAdjust.a = 0;
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B6))
            {
                GameSettings.JudgeAdjust.b -= 0.1f;
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B3))
            {
                GameSettings.JudgeAdjust.b += 0.1f;
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B5) || InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B4))
            {
                // Offset display. maybe don't reset to avoid accidental presses
                // AquaMai.AppConfig.Utils.JudgeAdjustB = 0;
            }
            else if (cancel)
            {
                PracticeMode.ui = null;
                Destroy(this);
            }
            // round off floating-point inaccuracies
            GameSettings.JudgeAdjust.a = (float)Math.Round(GameSettings.JudgeAdjust.a * 10) / 10;
            GameSettings.JudgeAdjust.b = (float)Math.Round(GameSettings.JudgeAdjust.b * 10) / 10;
        }
        else
        {
            if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.E8))
            {
                PracticeMode.Seek(-4000);
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.E2))
            {
                PracticeMode.Seek(4000);
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B8) || InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B1))
            {
                DebugFeature.Pause = !DebugFeature.Pause;
                if (!DebugFeature.Pause)
                {
                    PracticeMode.Seek(0);
                }
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B7) && PracticeMode.repeatStart == -1)
            {
                PracticeMode.repeatStart = PracticeMode.CurrentPlayMsec;
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B7) && PracticeMode.repeatEnd == -1)
            {
                PracticeMode.SetRepeatEnd(PracticeMode.CurrentPlayMsec);
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B2))
            {
                PracticeMode.ClearRepeat();
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B6))
            {
                PracticeMode.SpeedDown();
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B3))
            {
                PracticeMode.SpeedUp();
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B5) || InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.B4))
            {
                PracticeMode.SpeedReset();
            }
            else if (InputManager.GetTouchPanelAreaDown(InputManager.TouchPanelArea.E4))
            {
                PracticeMode.keepNoteSpeed = !PracticeMode.keepNoteSpeed;
                PracticeMode.gameCtrl?.ResetOptionSpeed();
            }
            else if (cancel)
            {
                PracticeMode.ui = null;
                Destroy(this);
            }
        }
    }
}
