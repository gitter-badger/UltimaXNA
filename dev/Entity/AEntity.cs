﻿/***************************************************************************
 *   BaseEntity.cs
 *      
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.View;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld.Model;
#endregion

namespace UltimaXNA.Entity
{
    public abstract class AEntity
    {
        // ============================================================
        // Properties
        // ============================================================

        public Serial Serial;

        public PropertyList PropertyList = new PropertyList();

        public bool IsDisposed = false;

        public bool IsClientEntity = false;

        public int Hue = 0;

        // ============================================================
        // Position
        // ============================================================

        public Map Map
        {
            get;
            private set;
        }

        private MapTile m_Tile;
        protected MapTile Tile
        {
            set
            {
                if (m_Tile != null)
                    m_Tile.OnExit(this);
                m_Tile = value;

                if (m_Tile != null)
                    m_Tile.OnEnter(this);
                else
                {
                    if (!IsDisposed)
                        Dispose();
                }
            }
        }

        private void OnTileChanged(int x, int y)
        {
            if (IsClientEntity && Map.Index >= 0)
                Map.CenterPosition = new Point(x, y);
            Tile = Map.GetMapTile(x, y);
        }

        public int Z
        {
            get { return Position.Z; }
        }

        private Position3D m_Position;
        public virtual Position3D Position { get { return m_Position; } }

        // ============================================================
        // Methods
        // ============================================================

        public AEntity(Serial serial, Map map)
        {
            Serial = serial;
            Map = map;

            m_Position = new Position3D(OnTileChanged);
        }

        public virtual void Update(double frameMS)
        {
            if (IsDisposed)
                return;

            InternalUpdateOverheads(frameMS);
        }

        public virtual void Dispose()
        {
            IsDisposed = true;
            Tile = null;
        }

        public override string ToString()
        {
            return Serial.ToString();
        }

        // ============================================================
        // Draw handling code
        // ============================================================

        private EntityViews.AEntityView m_View = null;

        protected virtual EntityViews.AEntityView CreateView()
        {
            return null;
        }

        public EntityViews.AEntityView GetView()
        {
            if (m_View == null)
                m_View = CreateView();
            return m_View;
        }

        internal virtual void Draw(MapTile tile, Position3D position)
        {

        }

        // ============================================================
        // Overhead handling code (labels, chat, etc.)
        // ============================================================

        private List<Overhead> m_Overheads = new List<Overhead>();
        public List<Overhead> Overheads
        {
            get { return m_Overheads; }
        }

        public Overhead AddOverhead(MessageType msgType, string text, int fontID, int hue)
        {
            Overhead overhead;

            for (int i = 0; i < m_Overheads.Count; i++)
            {
                overhead = m_Overheads[i];
                // is this overhead already active?
                if ((overhead.Text == text) && (overhead.MessageType == msgType) && !(overhead.IsDisposed))
                {
                    // reset the timer for the object so it lasts longer.
                    overhead.ResetTimer();
                    // update hue?
                    overhead.Hue = hue;
                    // insert it at the bottom of the queue so it displays closest to the player.
                    m_Overheads.RemoveAt(i);
                    m_Overheads.Insert(0, overhead);
                    return overhead;
                }
            }

            overhead = new Overhead(this, msgType, text);
            m_Overheads.Insert(0, overhead);
            return overhead;
        }

        internal void InternalDrawOverheads(MapTile tile, Position3D position)
        {
            // base entities do not draw, but they can have overheads, so we draw those.
            foreach (Overhead overhead in m_Overheads)
            {
                if (!overhead.IsDisposed)
                    overhead.Draw(tile, position);
            }
        }

        private void InternalUpdateOverheads(double frameMS)
        {
            // update overheads
            foreach (Overhead overhead in m_Overheads)
            {
                overhead.Update(frameMS);
            }
            // remove disposed of overheads.
            for (int i = 0; i < m_Overheads.Count; i++)
            {
                if (m_Overheads[i].IsDisposed)
                {
                    m_Overheads.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}