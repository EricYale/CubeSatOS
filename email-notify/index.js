const nodemailer = require("nodemailer");
const { networkInterfaces } = require("os");
const { email, appPassword } = require("./secrets.js");

const smtpTransport = nodemailer.createTransport({
	service: "gmail",
	auth: {
		user: email,
		pass: appPassword,
	},
});

let text = "No wlan0 network interface found.";
const interfaces = networkInterfaces();
if(interfaces.wlan0) {
	text = interfaces.wlan0.reduce((v, val) => {
		return (v + val.address + ",\t");
	}, "");
}

const mailOptions = {
	from: email,
	to: email,
	subject: "CubeSat up at " + (new Date()).toISOString(),
	text: `Addresses: ${text}`,
};
console.log(mailOptions.subject);
console.log(mailOptions.text);

smtpTransport.sendMail(mailOptions, (err, resp) => {
	if(err) {
		console.err(err);
		return;
	}
	console.log(resp);
	console.log("Sent email");
});
