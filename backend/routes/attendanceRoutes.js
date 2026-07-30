const express = require('express');
const router = express.Router();
const { clockIn, clockOut, getTodayStatus } = require('../controllers/attendanceController');
const { verifyToken } = require('../middleware/auth');

router.use(verifyToken);

router.post('/clock-in', clockIn);
router.post('/clock-out', clockOut);
router.get('/today', getTodayStatus);

module.exports = router;