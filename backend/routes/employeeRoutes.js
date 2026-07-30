const express = require('express');
const router = express.Router();
const {
    listEmployees, getEmployeeById, getEmployeeAttendance,
    updateEmployee, archiveEmployee
} = require('../controllers/employeeController');
const { verifyToken } = require('../middleware/auth');
const { requireRole } = require('../middleware/roleCheck');

router.use(verifyToken);

router.get('/', requireRole('Admin', 'HR', 'Manager'), listEmployees);
router.get('/:id', requireRole('Admin', 'HR', 'Manager'), getEmployeeById);
router.get('/:id/attendance', requireRole('Admin', 'HR', 'Manager'), getEmployeeAttendance);
router.put('/:id', requireRole('Admin', 'HR'), updateEmployee);
router.delete('/:id', requireRole('Admin', 'HR'), archiveEmployee);

module.exports = router;