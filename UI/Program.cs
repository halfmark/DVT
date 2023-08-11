using System.ComponentModel;
using System.Xml.Linq;
using DVT.BL.Implemantation;
using DVT.BL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using JasperFx.CodeGeneration.Frames;
using UI.Interface;
using UI;


// Create an instance of IConfigurationBuilder
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Build the configuration
var configuration = configBuilder.Build();

var container = new Lamar.Container(
    x => { 
        x.AddTransient<IElevatorService, ElevatorService>();
        x.AddTransient<IControls, ControlsService>();
        x.AddTransient<IStartProgram, StartProgram>();

    });

using (var scope = container)
{
    var myService = scope.GetInstance<IStartProgram>();
     myService.ConfigureThenRun();
}



