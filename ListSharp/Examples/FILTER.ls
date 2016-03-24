ROWS nums = {"1","2","3","11","22","33","111","222","333"}

ROWS temp = FILTER nums IF CONTAINS ["1"]
SHOW = temp

ROWS temp = FILTER nums IF CONTAINSNOT ["1"]
SHOW = temp

ROWS temp = FILTER nums IF IS ["1"]
SHOW = temp

ROWS temp = FILTER nums IF ISNOT ["1"]
SHOW = temp