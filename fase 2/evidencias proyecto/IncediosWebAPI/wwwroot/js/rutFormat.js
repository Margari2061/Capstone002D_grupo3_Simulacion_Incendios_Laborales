const rutFormat = function () {

    const approved = '0123456789kK';

    const initialize = () => {

        const inputs = document.querySelectorAll('input[type=rut]');

        inputs.forEach((i) => {
            i.addEventListener('input', (ev) => behaviour(ev));
        });
    }

    const behaviour = (ev) => {
        ev.target.value = format(ev.target.value);
    }

    const isApproved = (c) => {
        if (approved.indexOf(c) > -1)
            return true;
        return false;
    }

    const format = (v) => {

        v = v.replace('.', '');
        v = v.replace('-', '');

        for (let i = 0; i < v.length; i++) {
            if (!isApproved(v[i]))
                v = v.replace(v[i], '');
        }

        if (v.length > 1) {
            hyphenPos = v.length - 1;
            v = v.slice(0, hyphenPos) + '-' + v.slice(hyphenPos);
        }

        dotIndex = 5;
        while (v.length > dotIndex) {
            dotPos = v.length - dotIndex;
            v = v.slice(0, dotPos) + '.' + v.slice(dotPos);
            dotIndex = dotIndex + 4;
        }

        v = v.substring(v.length - 13);

        return v;
    }

    return {
        init: () => {
            initialize();
        }
    }

}();