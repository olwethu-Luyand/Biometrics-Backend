const pool = require('../config/db');

exports.clockIn = async (req, res) => {
    const userId = req.user.userId;
    const { method = 'Web' } = req.body;
    const today = new Date().toISOString().split('T')[0];

    try {
        const existing = await pool.query(
            'SELECT id, clock_in, clock_out FROM attendance WHERE user_id = $1 AND date = $2',
            [userId, today]
        );

        if (existing.rows.length > 0) {
            const record = existing.rows[0];
            if (record.clock_in && !record.clock_out) {
                return res.status(409).json({ message: 'Already clocked in. Clock out before clocking in again.' });
            }
            if (record.clock_in && record.clock_out) {
                return res.status(409).json({ message: 'Already completed attendance for today.' });
            }
        }

        const now = new Date();
        const scheduledStartHour = 9;
        const status = now.getHours() > scheduledStartHour ||
            (now.getHours() === scheduledStartHour && now.getMinutes() > 15)
            ? 'Late' : 'Present';

        const result = await pool.query(
            `INSERT INTO attendance (user_id, date, clock_in, clock_in_method, status)
       VALUES ($1, $2, $3, $4, $5)
       ON CONFLICT (user_id, date)
       DO UPDATE SET clock_in = $3, clock_in_method = $4, status = $5
       RETURNING id, date, clock_in, status`,
            [userId, today, now, method, status]
        );

        res.status(201).json({ message: 'Clocked in.', attendance: result.rows[0] });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Clock-in failed.' });
    }
};

exports.clockOut = async (req, res) => {
    const userId = req.user.userId;
    const { method = 'Web' } = req.body;
    const today = new Date().toISOString().split('T')[0];

    try {
        const existing = await pool.query(
            'SELECT id, clock_in, clock_out FROM attendance WHERE user_id = $1 AND date = $2',
            [userId, today]
        );

        if (existing.rows.length === 0 || !existing.rows[0].clock_in) {
            return res.status(400).json({ message: 'You must clock in before clocking out.' });
        }

        if (existing.rows[0].clock_out) {
            return res.status(409).json({ message: 'Already clocked out for today.' });
        }

        const now = new Date();
        const clockInTime = new Date(existing.rows[0].clock_in);
        const hoursWorked = ((now - clockInTime) / (1000 * 60 * 60)).toFixed(2);

        const result = await pool.query(
            `UPDATE attendance
       SET clock_out = $1, clock_out_method = $2, hours_worked = $3
       WHERE user_id = $4 AND date = $5
       RETURNING id, date, clock_in, clock_out, hours_worked, status`,
            [now, method, hoursWorked, userId, today]
        );

        res.json({ message: 'Clocked out.', attendance: result.rows[0] });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Clock-out failed.' });
    }
};

exports.getTodayStatus = async (req, res) => {
    const userId = req.user.userId;
    const today = new Date().toISOString().split('T')[0];

    try {
        const result = await pool.query(
            'SELECT id, clock_in, clock_out, status, hours_worked FROM attendance WHERE user_id = $1 AND date = $2',
            [userId, today]
        );

        res.json({ attendance: result.rows[0] || null });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: 'Failed to fetch status.' });
    }
};