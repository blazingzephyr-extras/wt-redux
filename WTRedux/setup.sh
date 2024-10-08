
# Exit immediately on error
set -e
cd $1

if [ -d "fnalibs3" ]; then
  echo -e "\e[32mfnalibs3 have already been downloaded before.\e[m"
else
  mkdir -p fnalibs3
  cd fnalibs3

  echo -e "\e[32mDownloading fnalibs3...\e[m"
  curl -O https://fna.flibitijibibo.com/archive/fnalibs3.tar.bz2

  echo -e "\e[32mExtracting fnalibs3...\e[m"
  tar -xf fnalibs3.tar.bz2
fi
