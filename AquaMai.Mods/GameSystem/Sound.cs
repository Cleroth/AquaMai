using System;
using AquaMai.Config.Attributes;
using HarmonyLib;
using Manager;
using MelonLoader;

namespace AquaMai.Mods.GameSystem;

[ConfigSection(
    zh: "音频独占与八声道设置",
    en: "Audio Exclusive and 8-Channel Settings")]
public static class Sound
{
    [ConfigEntry(
        zh: "是否启用音频独占",
        en: "Enable Audio Exclusive")]
    private readonly static bool enableExclusive = false;

    [ConfigEntry(
        zh: "是否启用八声道",
        en: "Enable 8-Channel")]
    private readonly static bool enable8Channel = false;

	[ConfigEntry(
		 zh: "",
		 en: "Samples per second. Higher values reduce audio latency (96 kHz, 192 kHz, ...)")]
	 private readonly static uint samplesPerSec = 48000u;

	[ConfigEntry(
		 zh: "",
		 en: "Buffer Time (in 100-ns ticks). Set it to the lowest value where you don't hear any audio glitches to improve latency.")]
	private readonly static uint bufferTime = 160000u;

	private static CriAtomUserExtension.AudioClientShareMode AudioShareMode => enableExclusive ? CriAtomUserExtension.AudioClientShareMode.Exclusive : CriAtomUserExtension.AudioClientShareMode.Shared;

    private const ushort wBitsPerSample = 32;
    private static ushort nChannels => enable8Channel ? (ushort)8 : (ushort)2;
    private static ushort nBlockAlign => (ushort)(wBitsPerSample / 8 * nChannels);
    private static uint nAvgBytesPerSec => samplesPerSec * nBlockAlign;

    private static CriAtomUserExtension.WaveFormatExtensible CreateFormat() =>
        new()
        {
            Format = new CriAtomUserExtension.WaveFormatEx
            {
                wFormatTag = 65534,
                nSamplesPerSec = samplesPerSec,
                wBitsPerSample = wBitsPerSample,
                cbSize = 22,
                nChannels = nChannels,
                nBlockAlign = nBlockAlign,
                nAvgBytesPerSec = nAvgBytesPerSec
            },
            Samples = new CriAtomUserExtension.Samples
            {
                wValidBitsPerSample = 24,
            },
            dwChannelMask = enable8Channel ? 1599u : 3u,
            SubFormat = new Guid("00000001-0000-0010-8000-00aa00389b71")
        };

    [HarmonyPrefix]
    // Original typo
    [HarmonyPatch(typeof(WasapiExclusive), "Intialize")]
    public static bool InitializePrefix()
    {
        CriAtomUserExtension.SetAudioClientShareMode(AudioShareMode);
		  // double num = (double)nBufferSize * (double)nChannels;
		  // ulong bufferTime = (ulong)Math.Ceiling((double)nBufferSize / num * 1000.0);
		  CriAtomUserExtension.SetAudioBufferTime(bufferTime);
		  // CriAtomUserExtension.SetAudioBufferTime(1);
		  // CriAtomUserExtension.SetAudioBufferTime(160000uL);

		  var format = CreateFormat();
        CriAtomUserExtension.SetAudioClientFormat(ref format);
        return false;
    }
}
