const express = require('express');
const router = express.Router();
const { registerEmployee, login, forgotPassword, resetPassword } = require('../controllers/authController');
const { verifyToken } = require('../middleware/auth');
const { requireRole } = require('../middleware/roleCheck');

router.post('/login', login);
router.post('/forgot-password', forgotPassword);
router.post('/reset-password', resetPassword);
router.post('/register', verifyToken, requireRole('Admin', 'HR'), registerEmployee);

module.exports = router;