var path = require('path'),
    rootPath = path.normalize(__dirname + '/..'),
    env = process.env.NODE_ENV || 'development';

var config = {
  development: {
    root: rootPath,
    app: {
      name: 'shippingservice'
    },
    port: process.env.PORT || 3000,
    db: 'postgres://pguser:skhokho@localhost/shippingservice-development'
   
  },

  test: {
    root: rootPath,
    app: {
      name: 'shippingservice'
    },
    port: process.env.PORT || 3000,
    db: 'postgres://postgres:skhokho@localhost/shippingservice-development'
  },

  production: {
    root: rootPath,
    app: {
      name: 'shippingservice'
    },
    port: process.env.PORT || 3000,
    db: 'postgres://localhost/shippingservice-production',
     
  }
};

module.exports = config[env];
