using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dependencies;
using CoveoBCC.Core;
using CoveoBCC.Interfaces;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using Unity.WebApi;

namespace CoveoBCC.Web
{
    public class Startup
    {
        public static void ConfigureContainer( IUnityContainer container )
        {
            container.RegisterType<INameIndexFactory, CoveoBCC.Core.Searcher.NameSearcherFactory>( new ContainerControlledLifetimeManager() );
            container.RegisterType<ICityRepository, FileCityRepository>( new ContainerControlledLifetimeManager() );
            container.RegisterType<ISuggestionsProvider, CoveoBCC.Core.Core>( new ContainerControlledLifetimeManager() ); ;
            container.RegisterInstance<string>( "CitiesTSVFile", @"C:\Users\matzoliv\dev\backend-coding-challenge\data\cities_canada-usa.tsv" );
        }

        public void Configuration ( IAppBuilder app )
        {
            HttpConfiguration config = new HttpConfiguration();

            var defaultSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter{ CamelCaseText = true }, }
            };

            JsonConvert.DefaultSettings = () => { return defaultSettings; };

            config.Formatters.Clear();
            config.Formatters.Add( new JsonMediaTypeFormatter() );
            config.Formatters.JsonFormatter.SerializerSettings = defaultSettings;

            config.Routes.MapHttpRoute( "Default", "{controller}/{id}", new { id = RouteParameter.Optional } );

            var container = new UnityContainer();

            ConfigureContainer( container );

            config.DependencyResolver = new UnityDependencyResolver( container );

            app.UseWebApi( config );
        }
    }
}
