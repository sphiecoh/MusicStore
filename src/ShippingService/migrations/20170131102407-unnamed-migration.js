'use strict';

module.exports = {
  up: function (queryInterface, Sequelize) {
    queryInterface.addColumn({
    tableName: 'Shipments',
    schema: 'public'
  },
  'userid',
  Sequelize.STRING
)
  },

  down: function (queryInterface, Sequelize) {
    /*
      Add reverting commands here.
      Return a promise to correctly handle asynchronicity.

      Example:
      return queryInterface.dropTable('users');
    */
  }
};
