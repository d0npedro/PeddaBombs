using CatCore.Services.Multiplexer;
using PeddaBombs.Configuration;
using PeddaBombs.Interfaces;
using PeddaBombs.Models;
using PeddaBombs.Staics;
using UnityEngine;

namespace PeddaBombs.CommandControllers
{
    public class BombCommandController : MonoBehaviour, ICommandable
    {
        public string Key => CommandKey.BOMB;
        public void Execute(MultiplexedPlatformService service, MultiplexedMessage message)
        {
            if (PluginConfig.Instance.IsBombEnable != true) {
                return;
            }
            DummyBomb.Senders.Enqueue(message.Sender.DisplayName);
        }
    }
}
