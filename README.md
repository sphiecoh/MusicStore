# Music Store

[![Build status](https://ci.appveyor.com/api/projects/status/2k0l47ej8ohle5re/branch/master?svg=true)](https://ci.appveyor.com/project/sphiecoh/musicstore/branch/master)

This is a simple project build by Nancyfx .

Run on dotnet core (Kestrel).

What technologies this project use are as follow:

- Nancyfx
- Dapper
- Npgsql

To run this project :
- `cd db`
- `pg_restore -f nancymusicstore.tar -v --create`
- `cd` into src\NancyMusicStore
- `dotnet restore`
- `dotnet run `
- `cd` src\ShippingService
- `npm install`
- `node app.js`

Adopted from https://github.com/hwqdt/NancyMusicStore.

Here is the gif picture to show this project.

![demonstration](https://raw.githubusercontent.com/hwqdt/NancyMusicStore/master/demonstration.gif)
