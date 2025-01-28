/* #region Definitions / Requirements */
const EXPRESS = require('express');
const APP = EXPRESS();
const PORT = 8887;
const BODY_PARSER = require('body-parser');
const CORS = require('cors');
const MONGOOSE = require('mongoose');
/* #endregion */

/* #region Middleware */
APP.use(BODY_PARSER.json());
APP.use(CORS());
/* #endregion */

/* #region MongoDB Connection */
const DATABASE_URL = 'mongodb://0.0.0.0:27017/sheridan';
MONGOOSE.set('strictQuery', true);
MONGOOSE.connect(DATABASE_URL, { useNewUrlParser: true, useUnifiedTopology: true })
    .then(() => console.log('MongoDB connected successfully'))
    .catch(err => console.error('MongoDB connection error:', err));
/* #endregion */

/* #region Server Start */
APP.listen(PORT, () => {
    console.log(`Server running at http://localhost:${PORT}!`);
});
/* #endregion */