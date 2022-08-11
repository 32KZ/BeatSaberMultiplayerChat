﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiplayerChat.Config;
using SiraUtil.Logging;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace MultiplayerChat.Audio;

/// <summary>
/// Utility for playing notification sounds from audio clips.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class SoundNotifier : MonoBehaviour, IInitializable, IDisposable
{
    [Inject] private readonly SiraLog _log = null!;
    [Inject] private readonly PluginConfig _config = null!;

    private readonly string _directoryPath;
    private readonly Dictionary<string, AudioClip> _loadedClips;
    private AudioSource? _audioSource;
    private bool _previewMode;

    public SoundNotifier()
    {
        _directoryPath = Environment.CurrentDirectory + "/UserData/MultiplayerChat";
        _loadedClips = new();
        _audioSource = null;
        _previewMode = false;
    }

    #region Init

    public void Initialize()
    {
        try
        {
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);
        }
        catch (Exception)
        {
            // I/O error
        }
    }

    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        _previewMode = false;

        LoadConfiguredClip();
    }

    public void Dispose()
    {
        if (_audioSource != null)
            _audioSource.Stop();

        foreach (var clip in _loadedClips.Values)
            if (clip != null)
                Destroy(clip);

        _loadedClips.Clear();
    }

    #endregion

    #region Playback

    public void Play()
    {
        if (string.IsNullOrEmpty(_config.SoundNotification))
            return;

        Play(_config.SoundNotification!);
    }

    public void Play(string clipName)
    {
        if (clipName == "None")
            return;

        _previewMode = false;

        if (_audioSource is null)
            return;

        if (!clipName.EndsWith(".ogg"))
            clipName += ".ogg";

        if (!_loadedClips.TryGetValue(clipName, out var audioClip))
        {
            _log.Warn($"Can't play audio clip because it's not loaded: {clipName}");
            return;
        }

        _audioSource.PlayOneShot(audioClip, _config.SoundNotificationVolume);
    }

    public void LoadClipIfNeeded(string clipName)
    {
        if (!isActiveAndEnabled)
            return;
        
        if (clipName == "None")
            return;
        
        if (!clipName.EndsWith(".ogg"))
            clipName += ".ogg";

        if (_loadedClips.ContainsKey(clipName))
            return;
        
        StartCoroutine(nameof(LoadClipRoutine), clipName);
    }

    public void LoadAndPlayPreview(string clipName)
    {
        if (!isActiveAndEnabled)
            return;
        
        if (clipName == "None")
            return;
        
        if (!clipName.EndsWith(".ogg"))
            clipName += ".ogg";

        if (_loadedClips.ContainsKey(clipName))
        {
            Play(clipName);
            return;
        }

        _previewMode = true;
        StartCoroutine(nameof(LoadClipRoutine), clipName);
    }

    #endregion

    #region Loading

    private void LoadConfiguredClip()
    {
        if (!isActiveAndEnabled)
            return;
        
        if (string.IsNullOrEmpty(_config.SoundNotification) || _config.SoundNotification == "None" ||
            _config.SoundNotificationVolume <= 0)
        {
            _log.Info("Sound notification is disabled in config");
            return;
        }

        StartCoroutine(nameof(LoadClipRoutine), _config.SoundNotification);
    }

    public IEnumerable<string> GetAvailableClipNames() =>
        Directory.EnumerateFiles(_directoryPath, "*.ogg", SearchOption.TopDirectoryOnly)
            .Select(path => new FileInfo(path).Name);


    private IEnumerator LoadClipRoutine(string clipName)
    {
        if (clipName == "None")
            yield break;
        
        if (!clipName.EndsWith(".ogg"))
            clipName += ".ogg";

        if (_loadedClips.TryGetValue(clipName, out var existingClip))
        {
            _log.Warn($"[LoadClipRoutine] Skipping duplicate clip: {clipName}");
            yield break;
        }

        var localPath = Path.Combine(_directoryPath, clipName);

        if (!File.Exists(localPath))
        {
            _log.Error($"[LoadClipRoutine] Sound name must refer to valid .ogg file on disk (tried {localPath})");
            yield break;
        }

        var request = UnityWebRequestMultimedia.GetAudioClip($"file://{localPath}", AudioType.OGGVORBIS);
        yield return request.SendWebRequest();

        if (!string.IsNullOrEmpty(request.error))
        {
            _log.Error($"[LoadClipRoutine] Error trying to load {localPath}: {request.error}");
            yield break;
        }

        var audioClip = DownloadHandlerAudioClip.GetContent(request);

        if (audioClip == null)
        {
            _log.Error($"[LoadClipRoutine] Error trying to load {localPath}: failed to get clip");
            yield break;
        }

        _loadedClips[clipName] = audioClip;
        _log.Debug($"Loaded clip: {clipName}");

        if (_previewMode)
            Play(clipName);
    }

    #endregion
}