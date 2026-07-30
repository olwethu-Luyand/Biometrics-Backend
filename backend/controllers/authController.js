const bcrypt = require('bcrypt');
const jwt = require('jsonwebtoken');
const crypto = require('crypto');
const pool = require('../config/db');

const SALT_ROUNDS = 12;

exports.registerEmployee = async (req, res) => {
    const {
        fullName, email, phone, password, role,
        department, jobTitle, employmentType,
        hireDate, workSchedule, managerId
    } = req.body;

    try {
        const existing = await pool.query('SELECT id FROM users WHERE email = $1', [email]);
        if (existing.rows.length > 0) {
            return res.status(409).json({ message: 'Email already registered.' });
        }

        const passwordHash = await bcrypt.hash(password, SALT_ROUNDS);

        const seqResult = await pool.query("SELECT nextval('employee_id_seq') AS seq");
        const employeeId = `EMP-${String(seqResult.rows[0].seq).padStart(4, '0')}`;

        const result = await pool.query(
            `INSERT INTO users
        (employee_id, full_name, email, phone, password_hash, role, department,
         job_title, employment_type, hire_date, work_schedule, manager_id)
       VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9,$10,$11,$12)
       RETURNING id, employee_id, full_name, email, role`,
            [employeeId, fullName, email, phone, passwordHash, role, department,
                jobTitle, employmentType, hireDate, workSchedule, managerId || null]
        );

        res.status(201).json({ message: 'Employee registered.', user: result.rows[0] });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Registration failed.' });
    }
};

exports.login = async (req, res) => {
    const { email, password } = req.body;

    try {
        const result = await pool.query('SELECT * FROM users WHERE email = $1 AND is_active = true', [email]);
        const user = result.rows[0];

        if (!user) {
            return res.status(401).json({ message: 'Invalid email or password.' });
        }

        const passwordMatch = await bcrypt.compare(password, user.password_hash);
        if (!passwordMatch) {
            return res.status(401).json({ message: 'Invalid email or password.' });
        }

        await pool.query('UPDATE users SET last_login = NOW() WHERE id = $1', [user.id]);

        const token = jwt.sign(
            { userId: user.id, role: user.role, employeeId: user.employee_id },
            process.env.JWT_SECRET,
            { expiresIn: process.env.JWT_EXPIRES_IN }
        );

        res.json({
            token,
            user: {
                id: user.id,
                employeeId: user.employee_id,
                fullName: user.full_name,
                email: user.email,
                role: user.role
            }
        });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Login failed.' });
    }
};

exports.forgotPassword = async (req, res) => {
    const { email } = req.body;

    try {
        const result = await pool.query('SELECT id FROM users WHERE email = $1', [email]);
        if (result.rows.length === 0) {
            return res.status(200).json({ message: 'If that email exists, a reset link has been sent.' });
        }

        const userId = result.rows[0].id;
        const token = crypto.randomBytes(32).toString('hex');
        const expiresAt = new Date(Date.now() + 30 * 60 * 1000);

        await pool.query(
            'INSERT INTO password_reset_tokens (user_id, token, expires_at) VALUES ($1, $2, $3)',
            [userId, token, expiresAt]
        );

        // send email with reset link containing `token`

        res.status(200).json({ message: 'If that email exists, a reset link has been sent.' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Request failed.' });
    }
};

exports.resetPassword = async (req, res) => {
    const { token, newPassword } = req.body;

    try {
        const result = await pool.query(
            `SELECT * FROM password_reset_tokens
       WHERE token = $1 AND used = false AND expires_at > NOW()`,
            [token]
        );

        if (result.rows.length === 0) {
            return res.status(400).json({ message: 'Invalid or expired reset token.' });
        }

        const resetRecord = result.rows[0];
        const passwordHash = await bcrypt.hash(newPassword, SALT_ROUNDS);

        await pool.query('UPDATE users SET password_hash = $1 WHERE id = $2', [passwordHash, resetRecord.user_id]);
        await pool.query('UPDATE password_reset_tokens SET used = true WHERE id = $1', [resetRecord.id]);

        res.json({ message: 'Password reset successful.' });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Reset failed.' });
    }
};