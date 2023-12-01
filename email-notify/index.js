const nodemailer = require("nodemailer");
const { networkInterfaces } = require("os");
const { email, appPassword } = require("./secrets.js");
const dns = require("dns");

function onNetworkConnect() {
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
}


function checkIsOnline(isOnline) {
	console.log("Checking online status...");
	dns.resolve("www.google.com", (err) => {
		if(err) setTimeout(checkIsOnline, 5000);
		else onNetworkConnect();
	});
}

checkIsOnline();
