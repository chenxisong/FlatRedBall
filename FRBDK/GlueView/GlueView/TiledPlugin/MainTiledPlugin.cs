﻿using FlatRedBall.Glue.RuntimeObjects;
using GlueView.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledPlugin
{
    [Export(typeof(GlueViewPlugin))]
    public class MainTiledPlugin : GlueViewPlugin
    {
        public override string FriendlyName
        {
            get { return "Tiled GlueView Plugin"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0); }
        }

        public override bool ShutDown(FlatRedBall.Glue.Plugins.Interfaces.PluginShutDownReason shutDownReason)
        {
            // remove it
            //ReferencedFileRuntimeList.FileManagers.Add(new object);
            return true;
        }

        public override void StartUp()
        {
            //ReferencedFileRuntimeList.FileManagers.Add(new object);
        }
    }
}
