﻿using Autofac;
using Data.Contracts;

namespace PlexRipper.Data;

public class DataModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<PlexRipperDbContext>().As<IPlexRipperDbContext>().AsSelf().InstancePerDependency();

        builder.RegisterType<PlexRipperDbContext>().As<IPlexRipperDbContextDatabase>().InstancePerDependency();

        builder.RegisterType<PlexRipperDbContextManager>().As<IPlexRipperDbContextManager>().InstancePerDependency();
    }
}
