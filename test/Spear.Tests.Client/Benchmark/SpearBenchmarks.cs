﻿using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spear.Consul;
using Spear.Core;
using Spear.Core.Micro;
using Spear.Protocol.Http;
using Spear.Protocol.Tcp;
using Spear.ProxyGenerator;
using Spear.Tests.Contracts;
using System;
using System.Threading.Tasks;
using Spear.Nacos;

namespace Spear.Tests.Client.Benchmark
{
    [MemoryDiagnoser]//内存评测
    public class SpearBenchmarks
    {
        private IServiceProvider _provider;
        private ITestContract _contract;

        [Params("shay", "123456")]
        public string Name;

        [GlobalSetup]
        public void Setup()
        {
            var services = new MicroBuilder()
                .AddMicroClient(builder =>
                {
                    builder.AddJsonCoder()
                        .AddSession()
                        .AddHttpProtocol()
                        .AddTcpProtocol()
                        //.AddConsul("http://192.168.0.231:8500")
                        .AddNacos(opt =>
                        {
                            opt.Host = "http://192.168.0.231:8848/";
                            opt.Tenant = "ef950bae-865b-409b-9c3b-bc113cf7bf37";
                        });
                });
            _provider = services.BuildServiceProvider();
            var proxy = _provider.GetService<IProxyFactory>();
            _contract = proxy.Create<ITestContract>();
        }

        [Benchmark]
        public async Task Notice()
        {
            await _contract.Notice(Name);
        }

        [Benchmark]
        public async Task<string> Get()
        {
            var result = await _contract.Get(Name);
            return result;
            //_provider.GetService<ILogger<SpearBenchmarks>>().LogInformation(result);
        }
    }
}
