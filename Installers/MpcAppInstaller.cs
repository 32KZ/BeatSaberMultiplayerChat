﻿using BeatSaberMultiplayerChat.Audio;
using BeatSaberMultiplayerChat.Core;
using Zenject;

namespace BeatSaberMultiplayerChat.Installers;

/// <summary>
/// Installer for App (global).
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class MpcAppInstaller : Installer
{
    public override void InstallBindings()
    {
        // Plugin
        Container.Bind<PluginConfig>().FromInstance(Plugin.Config).AsSingle();
        
        // Audio
        Container.BindInterfacesAndSelfTo<MicrophoneManager>().FromNewComponentOnNewGameObject().AsSingle();
        Container.BindInterfacesAndSelfTo<VoiceManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<SoundNotifier>().FromNewComponentOnNewGameObject().AsSingle();
        
        // Core
        Container.BindInterfacesAndSelfTo<ChatManager>().AsSingle();
    }
}