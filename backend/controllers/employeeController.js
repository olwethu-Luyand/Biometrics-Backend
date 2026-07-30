const pool = require('../config/db');

exports.listEmployees = async (req, res) => {
    const {
        search, department, role, employmentType,
        isActive = 'true', page = 1, limit = 20
    } = req.query;

    const offset = (page - 1) * limit;
    const conditions = [];
    const values = [];
    let paramIndex = 1;

    conditions.push(`is_active = $${paramIndex++}`);
    values.push(isActive === 'true');

    if (search) {
        conditions.push(`(full_name ILIKE $${paramIndex} OR email ILIKE $${paramIndex} OR employee_id ILIKE $${paramIndex})`);
        values.push(`%${search}%`);
        paramIndex++;
    }
    if (department) {
        conditions.push(`department = $${paramIndex++}`);
        values.push(department);
    }
    if (role) {
        conditions.push(`role = $${paramIndex++}`);
        values.push(role);
    }
    if (employmentType) {
        conditions.push(`employment_type = $${paramIndex++}`);
        values.push(employmentType);
    }

    const whereClause = conditions.length ? `WHERE ${conditions.join(' AND ')}` : '';

    try {
        const countResult = await pool.query(`SELECT COUNT(*) FROM users ${whereClause}`, values);

        values.push(limit, offset);
        const result = await pool.query(
            `SELECT id, employee_id, full_name, email, phone, role, department,
              job_title, employment_type, hire_date, work_schedule,
              manager_id, is_active, last_login
       FROM users
       ${whereClause}
       ORDER BY full_name ASC
       LIMIT $${paramIndex++} OFFSET $${paramIndex++}`,
            values
        );

        res.json({
            employees: result.rows,
            total: parseInt(countResult.rows[0].count, 10),
            page: parseInt(page, 10),
            limit: parseInt(limit, 10)
        });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Failed to fetch employees.' });
    }
};

exports.getEmployeeById = async (req, res) => {
    const { id } = req.params;

    try {
        const result = await pool.query(
            `SELECT u.id, u.employee_id, u.full_name, u.email, u.phone, u.role,
              u.department, u.job_title, u.employment_type, u.hire_date,
              u.work_schedule, u.is_active, u.last_login, u.created_at,
              m.full_name AS manager_name, m.employee_id AS manager_employee_id
       FROM users u
       LEFT JOIN users m ON u.manager_id = m.id
       WHERE u.id = $1`,
            [id]
        );

        if (result.rows.length === 0) {
            return res.status(404).json({ message: 'Employee not found.' });
        }

        res.json({ employee: result.rows[0] });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Failed to fetch employee.' });
    }
};

exports.getEmployeeAttendance = async (req, res) => {
    const { id } = req.params;
    const { startDate, endDate, page = 1, limit = 30 } = req.query;
    const offset = (page - 1) * limit;

    const conditions = ['user_id = $1'];
    const values = [id];
    let paramIndex = 2;

    if (startDate) {
        conditions.push(`date >= $${paramIndex++}`);
        values.push(startDate);
    }
    if (endDate) {
        conditions.push(`date <= $${paramIndex++}`);
        values.push(endDate);
    }

    try {
        const employeeCheck = await pool.query('SELECT id FROM users WHERE id = $1', [id]);
        if (employeeCheck.rows.length === 0) {
            return res.status(404).json({ message: 'Employee not found.' });
        }

        values.push(limit, offset);
        const result = await pool.query(
            `SELECT id, date, clock_in, clock_out, status, hours_worked, notes
       FROM attendance
       WHERE ${conditions.join(' AND ')}
       ORDER BY date DESC
       LIMIT $${paramIndex++} OFFSET $${paramIndex++}`,
            values
        );

        res.json({ attendance: result.rows, page: parseInt(page, 10), limit: parseInt(limit, 10) });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Failed to fetch attendance history.' });
    }
};

exports.updateEmployee = async (req, res) => {
    const { id } = req.params;
    const allowedFields = [
        'full_name', 'phone', 'department', 'job_title',
        'employment_type', 'hire_date', 'work_schedule', 'manager_id', 'role'
    ];

    const updates = [];
    const values = [];
    let paramIndex = 1;

    for (const field of allowedFields) {
        const camelField = field.replace(/_([a-z])/g, (_, c) => c.toUpperCase());
        if (req.body[camelField] !== undefined) {
            updates.push(`${field} = $${paramIndex++}`);
            values.push(req.body[camelField]);
        }
    }

    if (updates.length === 0) {
        return res.status(400).json({ message: 'No valid fields to update.' });
    }

    updates.push(`updated_at = NOW()`);
    values.push(id);

    try {
        const result = await pool.query(
            `UPDATE users SET ${updates.join(', ')}
       WHERE id = $${paramIndex} AND is_active = true
       RETURNING id, employee_id, full_name, email, role, department, job_title`,
            values
        );

        if (result.rows.length === 0) {
            return res.status(404).json({ message: 'Employee not found or already archived.' });
        }

        res.json({ message: 'Employee updated.', employee: result.rows[0] });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Update failed.' });
    }
};

exports.archiveEmployee = async (req, res) => {
    const { id } = req.params;

    try {
        const result = await pool.query(
            `UPDATE users SET is_active = false, updated_at = NOW()
       WHERE id = $1 AND is_active = true
       RETURNING id, employee_id, full_name`,
            [id]
        );

        if (result.rows.length === 0) {
            return res.status(404).json({ message: 'Employee not found or already archived.' });
        }

        res.json({ message: 'Employee archived.', employee: result.rows[0] });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Archive failed.' });
    }
};