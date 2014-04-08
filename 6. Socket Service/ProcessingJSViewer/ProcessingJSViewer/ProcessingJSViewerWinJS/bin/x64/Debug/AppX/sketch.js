localSketch.attachFunction = function (processing) {

    var totalParticlesLeft = 100;
    var totalParticlesRight = 100;

    var particlesLeft = new Array(totalParticlesLeft);
    var particlesRight = new Array(totalParticlesRight);

    var particleIndexLeft = 0;
    var particleIndexRight = 0;

    var width;
    var height;
    var diameter = 10;

    var colorEmitterRight;
    var colorEmitterLeft;

    var screenHandLX = 50;
    var screenHandLY = 50;
    var screenHandRX = 50;
    var screenHandRY = 50;

    var screenHandLTargetX = 50;
    var screenHandLTargetY = 50;
    var screenHandRTargetX = 50;
    var screenHandRTargetY = 50;

    var centerX = 100;
    var centerY = 100;

    var leftDX = 0;
    var leftDY = 0;

    var rightDX = 0;
    var rightDY = 0;

    processing.setup = function () {
        width = jsCanvasWidth;
        height = jsCanvasHeight;
        processing.size(width, height);
        centerX = width / 2;
        centerY = height / 2;
        colorEmitterLeft = processing.color(255, 0, 0);
        colorEmitterRight = processing.color(0, 255, 0);

        processing.noStroke();
        processing.smooth();
        processing.background(0);

        for (var i = 0; i < totalParticlesLeft; i++) {
            var particle = new Particle();
            particlesLeft[i] = particle;
        }

        for (var i = 0; i < totalParticlesRight; i++) {
            var particle = new Particle();
            particlesRight[i] = particle;
        }


    };

    processing.draw = function () {
        processing.fill(0, 20);
        processing.rect(0, 0, width, height);
        processing.updateParticles();
    };


    processing.updateParticles = function () {

        screenHandLTargetX = centerX + 1 * (handLX * centerX);
        screenHandLTargetY = centerY - 1 * (handLY * centerY);
        leftDX += (screenHandLTargetX - screenHandLX) * .2;
        leftDY += (screenHandLTargetY - screenHandLY) * .2;
        screenHandLX += leftDX;
        screenHandLY += leftDY;
        leftDX *= .9;
        leftDY *= .9;

        screenHandRTargetX = centerX + 1*(handRX * centerX);
        screenHandRTargetY = centerY - 1 * (handRY * centerY);
        rightDX += (screenHandRTargetX - screenHandRX) * .2;
        rightDY += (screenHandRTargetY - screenHandRY) * .2;
        screenHandRX += rightDX;
        screenHandRY += rightDY;
        rightDX *= .9;
        rightDY *= .9;

        // loop this based on depth of hand for more particles per cycle
        var nextParticleLeft = particlesLeft[particleIndexLeft];
        nextParticleLeft.reset(screenHandLX, screenHandLY, 255, leftDX, leftDY);
        if (++particleIndexLeft >= particlesLeft.length) particleIndexLeft = 0;
        

        var nextParticleRight = particlesRight[particleIndexRight];
        nextParticleRight.reset(screenHandRX, screenHandRY, 0, rightDX, rightDY);
        if (++particleIndexRight >= particlesRight.length) particleIndexRight = 0;


        // draw particles first so they're behind the emitter circles
        for (var i = 0; i < totalParticlesLeft; i++) {
            particlesLeft[i].update();
            particlesLeft[i].render();
        }

        for (var i = 0; i < totalParticlesRight; i++) {
            particlesRight[i].update();
            particlesRight[i].render();
        }

        // then draw hand circles
        processing.fill(colorEmitterLeft);
        processing.ellipse(screenHandLX, screenHandLY, diameter, diameter);
        processing.fill(colorEmitterRight);
        processing.ellipse(screenHandRX, screenHandRY, diameter, diameter);
    };

    function Particle()
    {
        this.x = 0.0;
        this.y = 0.0;
        this.vx = 0.0;
        this.vy = 0.0;
        this.r = 255;
        this.g = 255;
        this.b = 255;
        this.a = 255;
        this.color = processing.color(255, 255, 255,255 );
        this.life = 0;
    }

    Particle.prototype.update = function () {
        if (this.life > 0) {
            this.life--;
                this.x += this.vx;
                this.y += this.vy;
            if (this.life < 50) {
                this.vx += Math.random() * 2.0 - 1.0;
                this.vx *= .96;
                this.vy += Math.random() * 2.0 - 1.0;
                this.vy *= .96;
                this.a = 255 * (this.life / 50.0);
            }
        }
    }

    Particle.prototype.reset = function (_x, _y, _hand, _dx, _dy)
    {
        this.x = _x;
        this.y = _y;
        this.vx = _dx * .3; //Math.random() * 4 - 2;
        this.vy = _dy * .3; //Math.random() * 4 - 2;
        this.life = Math.random() * 50 + 100;
        this.g = processing.map(_x, 0, width, 0, 255);
        this.b = processing.map(_y, 0, width, 0, 255);
        this.r = _hand;
        this.a = 255;
    }

    Particle.prototype.render = function () {
        processing.fill(processing.color(this.r, this.g, this.b, this.a));
        processing.ellipse(this.x, this.y, 5, 5);
    }

}



