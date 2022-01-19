# Orchard Core Issue Reproduction

### [#11041](https://github.com/OrchardCMS/OrchardCore/issues/11041)

Registering Typed HttpClients in the Host context is fine.

https://github.com/johnrom/orchard-core-issue-reproduction/blob/master/OrchardCoreHttpClientTest.Web/Startup.cs#L11-L13

Registering Typed HttpClients in the Module context is broken.

https://github.com/johnrom/orchard-core-issue-reproduction/blob/master/OrchardCoreHttpClientTest.Module/Startup.cs#L10-L11
