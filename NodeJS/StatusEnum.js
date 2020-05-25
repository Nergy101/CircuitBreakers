
module.exports = {
    OPEN: 1,
    HALFOPEN: 2,
    CLOSED: 3,
    SUCCESS: 4,
    // reverse-lookup function
    getKeyByValue: function (value) {
        return Object.keys(this).find(key => this[key] === value);
    }
};