using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.ContentManagement;
using RC.Engine.ContentManagement.ContentTypes;
using Microsoft.Xna.Framework;
using RC.Engine.GraphicsManagement;

namespace RC.Engine.Test
{
    class PotatoPool
    {
        List<Potato> _readyPool = new List<Potato>();
        List<Potato> _usedPool = new List<Potato>();
        RCSceneNode _root;

        public PotatoPool(IRCContentRequester content, int poolSizePotatoes, RCSceneNode root)
        {
            _root = root;
            for (int iPotato = 0; iPotato < poolSizePotatoes; iPotato++)
            {
                Potato p = new Potato(this, content);
                _root.AddChild(p);
                p.Body.DisableBody();
                _readyPool.Add(p);
                
            }
        }

        private Potato GetPotato()
        {
            Potato p = null;
            if (_readyPool.Count != 0)
            {
                p = _readyPool[0];
                _readyPool.RemoveAt(0);
                _usedPool.Insert(_usedPool.Count, p);
                p.Body.EnableBody();
            }
            else
            {

                p = _usedPool[0];
                _usedPool.RemoveAt(0);
                _usedPool.Insert(_usedPool.Count, p);
                p.Body.EnableBody();
            }

            return p;
        }

        public void FirePotato(Vector3 worldPos, Matrix worldOrient, Vector3 velocity)
        {
            Potato p = GetPotato();
            p.Fire(worldPos, worldOrient, velocity);
        }


        public void UsedPotatoExpired(Potato p)
        {
            System.Diagnostics.Debug.Assert(_usedPool.Remove(p));
            
            _readyPool.Insert(_readyPool.Count, p);
            p.Body.DisableBody();
        }
    }
}
