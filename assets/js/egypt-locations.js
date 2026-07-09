const EGYPT_LOCATIONS = {
  cairo: {
    label: 'Cairo',
    labelAr: 'القاهرة',
    cities: ['Nasr City', 'Maadi', 'Heliopolis', 'Zamalek', 'Shubra', 'Ain Shams', 'Mokattam', 'New Cairo', 'Helwan', 'Downtown']
  },
  giza: {
    label: 'Giza',
    labelAr: 'الجيزة',
    cities: ['Giza', '6th of October', 'Sheikh Zayed', 'Dokki', 'Mohandessin', 'Agouza', 'Haram', 'Faisal', 'Imbaba', 'Bulaq']
  },
  alexandria: {
    label: 'Alexandria',
    labelAr: 'الإسكندرية',
    cities: ['Alexandria', 'Borg El Arab', 'Montaza', 'Smouha', 'Miami', 'Agami', 'Stanley', 'Raml Station']
  },
  dakahlia: {
    label: 'Dakahlia',
    labelAr: 'الدقهلية',
    cities: ['Mansoura', 'Talkha', 'Mit Ghamr', 'Aga', 'Sinbillawain', 'Dekernes', 'Sherbin', 'Belqas']
  },
  'red-sea': {
    label: 'Red Sea',
    labelAr: 'البحر الأحمر',
    cities: ['Hurghada', 'Safaga', 'Marsa Alam', 'El Gouna', 'Quseir', 'Ras Ghareb', 'Shalateen']
  },
  beheira: {
    label: 'Beheira',
    labelAr: 'البحيرة',
    cities: ['Damanhour', 'Rashid', 'Edku', 'Abu Hummus', 'Itay El Barud', 'Kafr El Dawwar', 'Hosh Essa']
  },
  fayoum: {
    label: 'Fayoum',
    labelAr: 'الفيوم',
    cities: ['Fayoum', 'Tamiya', 'Sinnuris', 'Ibsheway', 'Itsa', 'Yusuf El Seddik']
  },
  gharbia: {
    label: 'Gharbia',
    labelAr: 'الغربية',
    cities: ['Tanta', 'Mahalla El Kubra', 'Zifta', 'Kafr El Zayat', 'Samanoud', 'Basyoun', 'Qutur']
  },
  ismailia: {
    label: 'Ismailia',
    labelAr: 'الإسماعيلية',
    cities: ['Ismailia', 'Fayed', 'Qantara', 'Abu Suwir', 'Tell El Kebir', 'Qassaseen']
  },
  monufia: {
    label: 'Monufia',
    labelAr: 'المنوفية',
    cities: ['Shibin El Kom', 'Menouf', 'Ashmoun', 'Quesna', 'Tala', 'Berket El Sabaa', 'Sadat City']
  },
  minya: {
    label: 'Minya',
    labelAr: 'المنيا',
    cities: ['Minya', 'Mallawi', 'Samalut', 'Maghagha', 'Beni Mazar', 'Matai', 'Abu Qurqas']
  },
  qalyubia: {
    label: 'Qalyubia',
    labelAr: 'القليوبية',
    cities: ['Banha', 'Qalyub', 'Shubra El Kheima', 'Khanka', 'Khosous', 'Qaha', 'Shebin El Qanater']
  },
  'new-valley': {
    label: 'New Valley',
    labelAr: 'الوادي الجديد',
    cities: ['Kharga', 'Dakhla', 'Farafra', 'Balat', 'Mut']
  },
  suez: {
    label: 'Suez',
    labelAr: 'السويس',
    cities: ['Suez', 'Arbaeen', 'Faisal', 'Ataqah', 'Ganayen']
  },
  aswan: {
    label: 'Aswan',
    labelAr: 'أسوان',
    cities: ['Aswan', 'Kom Ombo', 'Edfu', 'Daraw', 'Nasr El Nuba', 'Abu Simbel']
  },
  assiut: {
    label: 'Assiut',
    labelAr: 'أسيوط',
    cities: ['Assiut', 'Dayrout', 'Manfalut', 'Qusiya', 'Abnoub', 'Abu Tig', 'El Badari']
  },
  'beni-suef': {
    label: 'Beni Suef',
    labelAr: 'بني سويف',
    cities: ['Beni Suef', 'Biba', 'El Wasta', 'Nasser', 'Samasta', 'Fashn']
  },
  'port-said': {
    label: 'Port Said',
    labelAr: 'بورسعيد',
    cities: ['Port Said', 'Port Fouad', 'Arab District', 'Zohour District', 'Ganoub District']
  },
  damietta: {
    label: 'Damietta',
    labelAr: 'دمياط',
    cities: ['Damietta', 'New Damietta', 'Ras El Bar', 'Faraskur', 'Kafr Saad', 'Zarqa']
  },
  sharqia: {
    label: 'Sharqia',
    labelAr: 'الشرقية',
    cities: ['Zagazig', '10th of Ramadan', 'Belbeis', 'Abu Hammad', 'Minya El Qamh', 'Faqous', 'Hehia', 'Mashtool El Souk']
  },
  'south-sinai': {
    label: 'South Sinai',
    labelAr: 'جنوب سيناء',
    cities: ['Sharm El Sheikh', 'Dahab', 'Nuweiba', 'Saint Catherine', 'Tor Sinai', 'Abu Redis']
  },
  'kafr-el-sheikh': {
    label: 'Kafr El Sheikh',
    labelAr: 'كفر الشيخ',
    cities: ['Kafr El Sheikh', 'Desouk', 'Baltim', 'Fuwwah', 'Motobas', 'Qillin', 'Sidi Salem']
  },
  matrouh: {
    label: 'Matrouh',
    labelAr: 'مطروح',
    cities: ['Marsa Matrouh', 'El Alamein', 'Sidi Barrani', 'Siwa', 'Dabaa', 'Sallum']
  },
  luxor: {
    label: 'Luxor',
    labelAr: 'الأقصر',
    cities: ['Luxor', 'Esna', 'Armant', 'Tod', 'Qurna', 'Karnak']
  },
  qena: {
    label: 'Qena',
    labelAr: 'قنا',
    cities: ['Qena', 'Nag Hammadi', 'Dishna', 'Qus', 'Farshut', 'Abu Tesht']
  },
  'north-sinai': {
    label: 'North Sinai',
    labelAr: 'شمال سيناء',
    cities: ['Arish', 'Sheikh Zuweid', 'Rafah', 'Bir El Abd', 'Nakhl']
  },
  sohag: {
    label: 'Sohag',
    labelAr: 'سوهاج',
    cities: ['Sohag', 'Akhmim', 'Girga', 'Tahta', 'Tima', 'Maragha', 'El Balyana']
  }
};

function initEgyptLocationSelects(governorateId, cityId) {
  const governorateSelect = document.getElementById(governorateId);
  const citySelect = document.getElementById(cityId);
  if (!governorateSelect || !citySelect) return;

  governorateSelect.innerHTML = '<option value="" disabled selected>Select governorate</option>';
  Object.entries(EGYPT_LOCATIONS).forEach(([key, gov]) => {
    const option = document.createElement('option');
    option.value = key;
    option.textContent = `${gov.label} - ${gov.labelAr}`;
    governorateSelect.appendChild(option);
  });

  function resetCitySelect() {
    citySelect.innerHTML = '<option value="" disabled selected>Select city</option>';
    citySelect.disabled = true;
    citySelect.required = false;
  }

  resetCitySelect();

  governorateSelect.addEventListener('change', () => {
    const selected = EGYPT_LOCATIONS[governorateSelect.value];
    resetCitySelect();

    if (!selected) return;

    selected.cities.forEach(city => {
      const option = document.createElement('option');
      option.value = city;
      option.textContent = city;
      citySelect.appendChild(option);
    });

    citySelect.disabled = false;
    citySelect.required = true;
  });
}
