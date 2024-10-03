# Sitecore Item Access Override Module

This module provides a mechanism for overriding access typically assigned directly to items in the content tree. Through a configuration item you can now replace the effective access applied to items. The rules engine is leaveraged to evaluate when the access should be applied.

![image](https://github.com/michaellwest/security-override/assets/933163/fc7d9e85-eba5-4ffa-ad60-39ec6acdd399)

dotnet sitecore login --cm https://so-cm.dev.local --allow-write true --auth https://so-id.dev.local

## Contributing

* Run `init.ps1`
* Run `up.ps1`
* Run `package.ps1`
* Run `down.ps1`

**Note:** If for some reason docker doesn't end cleanly, such as through a reboot, you may need to run this:

```
docker network rm so_default
```