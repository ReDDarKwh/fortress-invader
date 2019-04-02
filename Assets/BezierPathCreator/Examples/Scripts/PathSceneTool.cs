﻿using UnityEngine;

namespace PathCreation.Examples
{

    public abstract class PathSceneTool : MonoBehaviour
    {
        public event System.Action onDestroyed;
        public PathCreator pathCreator;
        public bool autoUpdate = true;

        protected VertexPath path
        {
            get
            {
                return pathCreator.path;
            }
        }

        public void CreatePath()
        {
            if (pathCreator != null)
            {
                PathUpdated();
            }
        }

        protected virtual void OnDestroy()
        {
            if (onDestroyed != null)
            {
                onDestroyed();
            }
        }

        protected abstract void PathUpdated();
    }
}
